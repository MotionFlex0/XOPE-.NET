#include "namedpipeclient.h"

int testFop()  { return 2147683; }

NamedPipeClient::NamedPipeClient(const char* pipePath, std::shared_ptr<BS::thread_pool> pool)
{
    HANDLE h = CreateFileA(pipePath, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
    this->_pipe = h;
    this->_pool = pool;
}

void NamedPipeClient:: flushOutBuffer()
{
    if (isPipeBroken() || _outBuffer.empty())
        return;

    // Move messages out of queue all at once
    std::vector<std::unique_ptr<client::IMessage>> messages;
    {
        std::lock_guard<std::mutex> _outBufferLock(_mutex);
        while (!_outBuffer.empty())
        {
            messages.push_back(std::move(_outBuffer.front()));
            _outBuffer.pop();
        }
    }

    // Serialize message (convert to JSON and compress using zlib)
    std::vector<OutMessage_P> serializedMessages(messages.size());
    auto serializer = [this, &serializedMessages, &messages] (const uint64_t start, const uint64_t end)
    {
        for (uint64_t i = start; i < end; i++)
            serializedMessages[i] = std::move(serializeMessage(std::move(messages[i])));
    };

    if (_disablePool || _pool.expired() || true)
        serializer(0, messages.size());
    else
        _pool.lock()->parallelize_loop(messages.size(), serializer).wait();

    for (auto&& out : serializedMessages)
    {
        DWORD bytesWritten { 0 };
        BOOL res = WriteFile(_pipe, out->data.get(), out->length, &bytesWritten, NULL);
        if (!res)
        {
            DWORD lastError = GetLastError();
            if (lastError == ERROR_PIPE_NOT_CONNECTED || lastError == ERROR_BAD_PIPE)
            {
                pipeBroken = true;
                return;
            }
        }
    }
}

void NamedPipeClient::close()
{
    if (isPipeBroken())
        return;

    CloseHandle(_pipe);
    _pipe = INVALID_HANDLE_VALUE;
}

void NamedPipeClient::disableThreadPool(bool disable)
{
    _disablePool = disable;
}

bool NamedPipeClient::isPipeBroken()
{
    return _pipe == INVALID_HANDLE_VALUE || pipeBroken;
}

bool NamedPipeClient::send(std::unique_ptr<client::IMessage> mes)
{
    if (isPipeBroken())
        return false;

    std::lock_guard<std::mutex> lock(_mutex);
    _outBuffer.push(std::move(mes));

    return true;
}

NamedPipeClient::OutMessage_P NamedPipeClient::serializeMessage(std::unique_ptr<client::IMessage> message)
{
    DWORD bytesWritten{ 0 };
    json j;
    message->serializeToJson(j);
    // Convert JSON to CBOR
    std::vector<std::uint8_t> cbor = json::to_cbor(j);

    // Use zlib to compress data
    size_t upperBoundSize = compressBound(static_cast<uLong>(cbor.size()));
    std::vector<Bytef> compressedBuf(upperBoundSize);
    uLongf compressedBufSize = static_cast<uLongf>(compressedBuf.size());
    int err = compress(compressedBuf.data(), &compressedBufSize, cbor.data(), static_cast<uLong>(cbor.size()));
    x_assert(err == Z_OK, ("failed to compress bytes in NamedPipeClient::send. Error code: " + std::to_string(err)).c_str());

    // Move compressed data to a final buffer
    int len = static_cast<int>(compressedBufSize);
    auto buffer = std::make_unique<uint8_t[]>(len);
    memcpy(buffer.get(), compressedBuf.data(), len);

    auto outMessage = std::make_unique<OutMessage>();
    outMessage->data = std::move(buffer);
    outMessage->length = len;

    return outMessage;
}