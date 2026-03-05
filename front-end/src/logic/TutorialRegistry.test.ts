import { getStepsForRoute, TutorialRegistry } from './TutorialRegistry';

describe('TutorialRegistry', () => {
    it('should return steps for /dashboard', () => {
        const steps = getStepsForRoute('/dashboard');
        expect(steps.length).toBeGreaterThan(0);
        expect(steps[0].target).toBe('.dashboard-header');
    });

    it('should return steps for /jobs/new', () => {
        const steps = getStepsForRoute('/jobs/new');
        expect(steps.length).toBeGreaterThan(0);
        expect(steps[0].target).toBe('.job-form-container');
    });

    it('should return steps for /jobs/[id] (edit mode)', () => {
        const steps = getStepsForRoute('/jobs/123');
        expect(steps.length).toBeGreaterThan(0);
        expect(steps[0].target).toBe('.job-form-container');
    });

    it('should return empty array for unknown routes', () => {
        const steps = getStepsForRoute('/unknown');
        expect(steps).toEqual([]);
    });
});
