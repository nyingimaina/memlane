'use client';

import React from 'react';
import { ZestSidekickMenu } from 'jattac.libs.web.zest-sidekick-menu';
import { useRouter, usePathname } from 'next/navigation';

const Sidebar: React.FC = () => {
    const router = useRouter();
    const pathname = usePathname();

    const menuItems = [
        {
            id: 'dashboard',
            label: 'Dashboard',
            icon: 'dashboard',
            isActive: pathname === '/' || pathname === '/dashboard',
            onClick: () => router.push('/dashboard')
        },
        {
            id: 'jobs',
            label: 'Backup Jobs',
            icon: 'work',
            isActive: pathname.startsWith('/jobs'),
            onClick: () => router.push('/jobs')
        },
        {
            id: 'history',
            label: 'History',
            icon: 'history',
            isActive: pathname === '/history',
            onClick: () => router.push('/history')
        },
        {
            id: 'settings',
            label: 'Settings',
            icon: 'settings',
            isActive: pathname === '/settings',
            onClick: () => router.push('/settings')
        }
    ];

    return (
        <ZestSidekickMenu
            title="Memlane"
            items={menuItems}
            theme="corporate" // Assuming Zest components have theme options or CSS variable support
        />
    );
};

export default Sidebar;
