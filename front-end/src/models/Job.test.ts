import { JobStatus } from '@/models/Job';

describe('Job Model', () => {
    it('should have correct enum values', () => {
        expect(JobStatus.Pending).toBe(0);
        expect(JobStatus.InProgress).toBe(1);
        expect(JobStatus.Completed).toBe(2);
        expect(JobStatus.Failed).toBe(3);
        expect(JobStatus.Skipped).toBe(4);
    });
});
