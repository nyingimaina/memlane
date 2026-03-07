import { JobMetadata, JobRun } from '@/models/Job';
import { JobRepository } from '@/repositories/JobRepository';

export const JobService = {
    async getAll(): Promise<JobMetadata[]> {
        return await JobRepository.getAll();
    },

    async create(job: Partial<JobMetadata>): Promise<number> {
        return await JobRepository.create(job);
    },

    async update(id: number, job: Partial<JobMetadata>): Promise<void> {
        return await JobRepository.update(id, job);
    },

    async delete(id: number): Promise<void> {
        return await JobRepository.delete(id);
    },

    async trigger(id: number): Promise<void> {
        return await JobRepository.trigger(id);
    },

    async getRuns(jobId: number): Promise<JobRun[]> {
        return await JobRepository.getRuns(jobId);
    }
};
