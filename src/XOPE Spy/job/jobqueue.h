#pragma once
#include <future>
#include <mutex>
#include <functional>
#include "jobresponse.h"
#include "../utils/guid.h"
#include "../utils/concurrent_unordered_map.h"

class JobQueue
{
public:
	std::shared_ptr<JobResponse> push(Guid jobId);

	void completeJob(Guid guid, const IncomingMessage& im);

	void finishAllJobs();
	
private:
	ConcurrentUnorderedMap<Guid, std::shared_ptr<JobResponse>> _jobs;
};