#include "stringconverter.h"

std::string StringConverter::IpAddressV4ToString(const sockaddr_in* sin)
{
    char sourceAddr[INET_ADDRSTRLEN];
    int addrSize = sizeof(sourceAddr);
    int sinSize = sizeof(sockaddr_in);

    WSAAddressToStringA((LPSOCKADDR)sin, sinSize, NULL, sourceAddr, (LPDWORD)&addrSize);
    std::replace(sourceAddr, sourceAddr + sizeof(sourceAddr), ':', '\x00');
    return sourceAddr;
}

std::string StringConverter::IpAddressV6ToString(const sockaddr_in6* sin)
{
    char sourceAddr[INET6_ADDRSTRLEN];
    int addrSize = sizeof(sourceAddr);
    int sinSize = sizeof(sockaddr_in6);

    WSAAddressToStringA((LPSOCKADDR)sin, sinSize, NULL, sourceAddr, (LPDWORD)&addrSize);

    std::string formattedAddr{ sourceAddr };
    auto colonPos = formattedAddr.find_last_of(':');
    if (colonPos != std::string::npos)
        formattedAddr.erase(colonPos);
    return formattedAddr;
}

std::string StringConverter::ByteArrayToString(const uint8_t* byteArray, int length, char delimiter)
{
    std::stringstream ss;
    ss << std::setfill('0');

    for (int i = 0; i < length; i++)
    {
        int c = byteArray[i];
        ss << std::setw(2) << std::uppercase << std::hex << c;
        if (i < length - 1)
            ss << delimiter;
    }

    return ss.str();
}
