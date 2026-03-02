import axios from 'axios';
import { JobRepository } from './JobRepository';
import { JobStatus } from '@/models/Job';

jest.mock('axios');
const mockedAxios = axios as jest.Mocked<typeof axios>;

describe('JobRepository', () => {
    const mockJobs = [
        { id: 1, name: 'Job 1', type: 'Backup', status: JobStatus.Pending, createdAt: new Date().toISOString() }
    ];

    it('should fetch all jobs', async () => {
        mockedAxios.get.mockResolvedValue({ data: mockJobs });
        
        const result = await JobRepository.getAll();
        
        expect(result).toEqual(mockJobs);
        expect(mockedAxios.get).toHaveBeenCalledWith(`${process.env.NEXT_PUBLIC_API_URL}/jobs`);
    });

    it('should trigger a job', async () => {
        mockedAxios.post.mockResolvedValue({ status: 200 });
        
        await JobRepository.trigger(1);
        
        expect(mockedAxios.post).toHaveBeenCalledWith(`${process.env.NEXT_PUBLIC_API_URL}/jobs/1/trigger`);
    });
});
