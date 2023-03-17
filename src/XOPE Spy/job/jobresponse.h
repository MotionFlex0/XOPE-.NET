#pragma once
#include <condition_variable>
#include <mutex>
#include "../receiver/incomingmessage.h"
#include "../utils/guid.h"

class JobResponse
{
public:
	JobResponse(Guid jobId);

	const IncomingMessage& wait();

	void emitResponse(const IncomingMessage& response);
private:
	Guid _jobId;

	std::mutex _mutex;
	std::condition_variable _cv;
	IncomingMessage _response;
	bool _jobCompleted = false;
};