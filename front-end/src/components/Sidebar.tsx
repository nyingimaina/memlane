'use client';

import React from 'react';
import SidekickMenu from 'jattac.libs.web.zest-sidekick-menu';
import { useRouter, usePathname } from 'next/navigation';

const Sidebar: React.FC = () => {
    const router = useRouter();
    const pathname = usePathname();

    const menuItems = [
        {
            id: 'dashboard',
            label: 'Dashboard',
            icon: '📊',
            searchTerms: 'dashboard home statistics',
            onClick: () => router.push('/dashboard')
        },
        {
            id: 'jobs',
            label: 'Backup Jobs',
            icon: '💼',
            searchTerms: 'jobs backups configurations',
            onClick: () => router.push('/jobs')
        },
        {
            id: 'history',
            label: 'History',
            icon: '🕒',
            searchTerms: 'history logs previous',
            onClick: () => router.push('/history')
        },
        {
            id: 'settings',
            label: 'Settings',
            icon: '⚙️',
            searchTerms: 'settings preferences configuration',
            onClick: () => router.push('/settings')
        }
    ];

    return (
        <SidekickMenu
            items={menuItems}
            searchPlaceholder="Search Memlane..."
            openOnDesktop={true}
        />
    );
};

export default Sidebar;
