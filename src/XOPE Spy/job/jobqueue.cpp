#include "jobqueue.h"
#include "jobmessagetype.h"

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

void JobQueue::finishAllJobs()
{
	for (auto& [k, v] : _jobs)
	{
		IncomingMessage im;
		im.type = SpyMessageType::JOB_RESPONSE_DEFAULT;
		im.rawJsonData = 
		{ 
			{"Type", SpyMessageType::JOB_RESPONSE_DEFAULT},
		};
		v->emitResponse(im);
	}
	_jobs.clear();
}
