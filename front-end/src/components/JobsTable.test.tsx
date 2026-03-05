/* eslint-disable @typescript-eslint/no-explicit-any */
import React from 'react';
import { render, screen } from '@testing-library/react';
import JobsTable from './JobsTable';
import { JobMetadata, JobStatus } from '@/models/Job';

interface MockTableProps {
    columnDefinitions: {
        columnId: string;
        displayLabel: string;
        cellRenderer?: (data: any) => React.ReactNode;
    }[];
    data: any[];
}

// Mock components from external libraries
jest.mock('jattac.libs.web.responsive-table', () => {
    const MockTable = ({ columnDefinitions, data }: MockTableProps) => (
        <table>
            <thead>
                <tr>
                    {columnDefinitions.map((col) => (
                        <th key={col.columnId}>{col.displayLabel}</th>
                    ))}
                </tr>
            </thead>
            <tbody>
                {data.map((row, i) => (
                    <tr key={i}>
                        {columnDefinitions.map((col) => (
                            <td key={col.columnId}>
                                {col.cellRenderer ? col.cellRenderer(row) : (row as any)[col.columnId]}
                            </td>
                        ))}
                    </tr>
                ))}
            </tbody>
        </table>
    );
    MockTable.displayName = 'MockResponsiveTable';
    return MockTable;
});

jest.mock('jattac.libs.web.overflow-menu', () => {
    const MockMenu = () => <div data-testid="overflow-menu">Menu</div>;
    MockMenu.displayName = 'MockOverflowMenu';
    return MockMenu;
});

describe('JobsTable', () => {
    const mockJobs: JobMetadata[] = [
        {
            id: 1,
            name: 'Daily Backup',
            type: 'Backup',
            status: JobStatus.Completed,
            createdAt: new Date().toISOString(),
            lastRunAt: '2026-03-01T12:00:00Z',
            lastRunId: 42,
            healthScore: 100,
            totalRunsInWindow: 5,
            successCountInWindow: 5,
            nextRunAt: '2026-03-07T00:00:00Z'
        }
    ];

    it('renders monitoring columns correctly', () => {
        render(
            <JobsTable 
                jobs={mockJobs} 
                onTrigger={jest.fn()} 
                onEdit={jest.fn()} 
                onDelete={jest.fn()} 
                onShowHistory={jest.fn()} 
            />
        );

        // Check for column headers
        expect(screen.getByText('Health')).toBeInTheDocument();
        expect(screen.getByText('Last Run')).toBeInTheDocument();
        expect(screen.getByText('Next Run')).toBeInTheDocument();

        // Check for specific data
        expect(screen.getByText('#42')).toBeInTheDocument();
        expect(screen.getByTitle(/Sunny: Very stable \(5\/5 recent runs successful\)/)).toBeInTheDocument();
    });
});
