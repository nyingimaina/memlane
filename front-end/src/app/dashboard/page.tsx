'use client';

import React from 'react';
import { useJobLogic } from '@/logic/useJobLogic';
import JobsTable from '@/components/JobsTable';
import ZestButton from 'jattac.libs.web.zest-button';
import { FaPlus } from 'react-icons/fa6';

export default function DashboardPage() {
  const { jobs, isLoading, error, triggerJob } = useJobLogic();

  return (
    <div style={{ padding: '2rem' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
        <div>
          <h1 style={{ margin: 0 }}>Dashboard</h1>
          <p style={{ color: 'var(--secondary)', margin: 0 }}>Monitor and manage your automated backup jobs.</p>
        </div>
        <ZestButton 
          zest={{
            visualOptions: {
              variant: 'standard',
              iconLeft: <FaPlus />
            }
          }}
          onClick={() => console.log("New Job Clicked")}
        >
          New Backup Job
        </ZestButton>
      </div>

      {error && (
        <div className="card" style={{ borderLeft: '4px solid var(--danger)', marginBottom: '2rem', backgroundColor: 'var(--danger-bg)' }}>
          <p style={{ color: 'var(--danger)', margin: 0 }}>{error}</p>
        </div>
      )}

      {isLoading ? (
        <div style={{ display: 'flex', justifyContent: 'center', padding: '4rem' }}>
          <p>Loading your dashboard...</p>
        </div>
      ) : (
        <JobsTable 
          jobs={jobs} 
          onTrigger={triggerJob}
          onEdit={(job) => console.log("Edit", job)}
          onDelete={(id) => console.log("Delete", id)}
        />
      )}
    </div>
  );
}
