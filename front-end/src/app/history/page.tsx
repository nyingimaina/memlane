'use client';

import React from 'react';
import { useJobLogic } from '@/logic/useJobLogic';
import JobsTable from '@/components/JobsTable';
import { JobStatus } from '@/models/Job';

export default function HistoryPage() {
  const { jobs, isLoading, error, triggerJob } = useJobLogic();

  // Filter for completed, failed, or skipped jobs
  const historyJobs = jobs.filter(j => j.status !== JobStatus.Pending);

  return (
    <div style={{ padding: '2rem' }}>
      <div style={{ marginBottom: '2rem' }}>
        <h1 style={{ margin: 0 }}>Audit History</h1>
        <p style={{ color: 'var(--secondary)', margin: 0 }}>Review previous backup executions and their results.</p>
      </div>

      {error && (
        <div className="card" style={{ borderLeft: '4px solid var(--danger)', marginBottom: '2rem', backgroundColor: 'var(--danger-bg)' }}>
          <p style={{ color: 'var(--danger)', margin: 0 }}>{error}</p>
        </div>
      )}

      {isLoading ? (
        <div style={{ display: 'flex', justifyContent: 'center', padding: '4rem' }}>
          <p>Loading history...</p>
        </div>
      ) : (
        <div className="card" style={{ padding: '0' }}>
            {historyJobs.length > 0 ? (
                <JobsTable 
                    jobs={historyJobs} 
                    onTrigger={triggerJob}
                    onEdit={() => {}} // No edit in history
                    onDelete={() => {}} // No delete in history
                />
            ) : (
                <div style={{ padding: '4rem', textAlign: 'center', color: 'var(--secondary)' }}>
                    <p>No execution history found. Run a job to see results here.</p>
                </div>
            )}
        </div>
      )}
    </div>
  );
}
