'use client';

import React from 'react';
import SidekickMenu from 'jattac.libs.web.zest-sidekick-menu';
import { useRouter, usePathname } from 'next/navigation';
import { FaChartLine, FaBriefcase, FaClockRotateLeft, FaGear } from 'react-icons/fa6';

const Sidebar: React.FC = () => {
    const router = useRouter();
    const pathname = usePathname();

    const menuItems = [
        {
            id: 'dashboard',
            label: 'Dashboard',
            icon: <FaChartLine />,
            searchTerms: 'dashboard home statistics overview',
            onClick: () => router.push('/dashboard')
        },
        {
            id: 'jobs',
            label: 'Backup Jobs',
            icon: <FaBriefcase />,
            searchTerms: 'jobs backups configurations management',
            onClick: () => router.push('/jobs')
        },
        {
            id: 'history',
            label: 'History',
            icon: <FaClockRotateLeft />,
            searchTerms: 'history logs previous sessions audit',
            onClick: () => router.push('/history')
        },
        {
            id: 'settings',
            label: 'Settings',
            icon: <FaGear />,
            searchTerms: 'settings preferences configuration global',
            onClick: () => router.push('/settings')
        }
    ];

    return (
        <SidekickMenu
            items={menuItems}
            searchPlaceholder="Search Memlane..."
            openOnDesktop={false}
        />
    );
};

export default Sidebar;
