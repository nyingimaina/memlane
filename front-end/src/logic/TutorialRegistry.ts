import { Step } from 'react-joyride';

export interface TutorialRoute {
    route: string;
    steps: Step[];
}

export const TutorialRegistry: TutorialRoute[] = [
    {
        route: '/dashboard',
        steps: [
            {
                target: '.dashboard-header',
                content: 'Welcome to your Memlane Dashboard! Here you can monitor all your backup pipelines in real-time.',
                disableBeacon: true,
            },
            {
                target: '.jobs-summary-card',
                content: 'This area shows a quick summary of your active, pending, and failed jobs.',
            },
            {
                target: '.manual-trigger-btn',
                content: 'Need an immediate backup? Click here to trigger a job manually.',
            }
        ]
    },
    {
        route: '/jobs/new',
        steps: [
            {
                target: '.job-form-container',
                content: 'Let\'s configure your backup pipeline. Choose between Database or Directory backup types.',
                disableBeacon: true,
            },
            {
                target: '.job-name-field',
                content: 'Give your pipeline a clear, descriptive name.',
            },
            {
                target: '.database-provider-field',
                content: 'Select your database provider. Memlane will provide a connection string template for you.',
            },
            {
                target: '.connection-string-field',
                content: 'Fill in your specific database details into this template.',
            },
            {
                target: '.cron-schedule-field',
                content: 'Pick a predefined schedule or enter a custom Cron expression.',
            },
            {
                target: '.rotation-field',
                content: 'Specify how many old backups to keep to save disk space.',
            }
        ]
    }
];

export const getStepsForRoute = (pathname: string): Step[] => {
    if (pathname.startsWith('/jobs/')) {
        return TutorialRegistry.find(t => t.route === '/jobs/new')?.steps || [];
    }
    return TutorialRegistry.find(t => t.route === pathname)?.steps || [];
};
