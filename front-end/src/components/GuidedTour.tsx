'use client';

import React, { useEffect, useState } from 'react';
import Joyride, { CallBackProps, STATUS } from 'react-joyride';
import { usePathname } from 'next/navigation';
import { getStepsForRoute } from '@/logic/TutorialRegistry';

interface GuidedTourProps {
    manualTrigger?: boolean;
    onComplete?: () => void;
}

const GuidedTour: React.FC<GuidedTourProps> = ({ manualTrigger, onComplete }) => {
    const pathname = usePathname();
    const [run, setRun] = useState(false);
    const [steps, setSteps] = useState(getStepsForRoute(pathname));
    const [mounted, setMounted] = useState(false);

    useEffect(() => {
        setMounted(true);
    }, []);

    useEffect(() => {
        if (!mounted) return;
        setSteps(getStepsForRoute(pathname));
        
        // Check if user has seen this tour before
        const storageKey = `memlane_tour_${pathname}_seen`;
        const hasSeen = localStorage.getItem(storageKey);

        if (!hasSeen && steps.length > 0) {
            setRun(true);
        }
    }, [pathname, steps.length, mounted]);

    useEffect(() => {
        if (manualTrigger) {
            setRun(true);
        }
    }, [manualTrigger]);

    const handleJoyrideCallback = (data: CallBackProps) => {
        const { status } = data;
        const finishedStatuses: string[] = [STATUS.FINISHED, STATUS.SKIPPED];

        if (finishedStatuses.includes(status)) {
            setRun(false);
            const storageKey = `memlane_tour_${pathname}_seen`;
            localStorage.setItem(storageKey, 'true');
            if (onComplete) onComplete();
        }
    };

    if (!mounted || steps.length === 0) return null;

    return (
        <Joyride
            steps={steps}
            run={run}
            continuous={true}
            showProgress={true}
            showSkipButton={true}
            callback={handleJoyrideCallback}
            styles={{
                options: {
                    primaryColor: 'var(--accent)',
                    backgroundColor: 'var(--card-bg)',
                    textColor: 'var(--foreground)',
                    arrowColor: 'var(--card-bg)',
                    overlayColor: 'rgba(0, 0, 0, 0.5)',
                }
            }}
        />
    );
};

export default GuidedTour;
