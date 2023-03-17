#include "jobresponse.h"

JobResponse::JobResponse(Guid jobId) : _jobId(jobId)
{

}

const IncomingMessage& JobResponse::wait()
{
	if (_jobCompleted)
		return _response;

	std::unique_lock lock(_mutex);
	_cv.wait(lock, [&]{ return _jobCompleted; });
	return _response;
}

void JobResponse::emitResponse(const IncomingMessage& response)
{
	{
		std::lock_guard lock(_mutex);
		_response = response;
		_jobCompleted = true;
	}
	_cv.notify_one();
}
