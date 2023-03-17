#include "jobqueue.h"

std::shared_ptr<JobResponse> JobQueue::push(Guid jobId)
{
	std::shared_ptr<JobResponse> jobResponse = std::make_shared<JobResponse>(jobId);
	_jobs.tryAdd(jobId, jobResponse);
	return jobResponse;
}

void JobQueue::completeJob(Guid jobId, const IncomingMessage& im)
{
	if (_jobs.contains(jobId))
		_jobs[jobId]->emitResponse(im);
}
