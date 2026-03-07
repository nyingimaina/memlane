'use client';

import React, { useEffect, useCallback } from 'react';
import { useJobLogic } from '@/logic/useJobLogic';
import { useUI } from '@/logic/UIContext';
import JobsTable from '@/components/JobsTable';
import JobForm from '@/features/JobManagement/UI/JobForm';
import JobHistoryList from '@/components/JobHistoryList';
import RunLogViewer from '@/components/RunLogViewer';
import ZestButton from 'jattac.libs.web.zest-button';
import { FaPlus } from 'react-icons/fa6';
import { JobMetadata, JobRun } from '@/models/Job';
import { JobRepository } from '@/repositories/JobRepository';

export default function JobsPage() {
  const { jobs, isLoading, error, fetchJobs, triggerJob } = useJobLogic();
  const { openSidePane, closeSidePane } = useUI();

  const handleSubmit = useCallback(async (jobData: Partial<JobMetadata>, jobId?: number) => {
    try {
      if (jobId) {
        await JobRepository.update(jobId, jobData);
      } else {
        await JobRepository.create(jobData);
      }
      closeSidePane();
      await fetchJobs();
    } catch (err) {
      console.error("Failed to save job:", err);
      throw err; // For ZestButton feedback
    }
  }, [closeSidePane, fetchJobs]);

  const handleTrigger = async (id: number) => {
    try {
        await JobRepository.trigger(id);
        await fetchJobs();
    } catch (err) {
        console.error("Failed to trigger job:", err);
        throw err; // For ZestButton feedback
    }
  };

  const handleCreateNew = () => {
    openSidePane(
      <JobForm 
        onSubmit={(data) => handleSubmit(data)}
        onCancel={closeSidePane}
      />,
      "Create New Backup Job"
    );
  };

  const handleEdit = (job: JobMetadata) => {
    openSidePane(
      <JobForm 
        initialJob={job}
        onSubmit={(data) => handleSubmit(data, job.id)}
        onCancel={closeSidePane}
      />,
      `Edit: ${job.name}`
    );
  };

  const handleShowHistory = (job: JobMetadata) => {
    openSidePane(
        <JobHistoryList 
            jobId={job.id} 
            onSelectRun={(run) => handleShowRunDetail(run, job)} 
        />,
        `History: ${job.name}`,
        35
    );
  };

  const handleShowRunDetail = (run: JobRun, job: JobMetadata) => {
    openSidePane(
        <RunLogViewer run={run} />,
        `Log: ${job.name} (Run #${run.id})`,
        50
    );
  };

  const handleDelete = async (id: number) => {
    if (confirm("Are you sure you want to delete this job?")) {
      try {
        await JobRepository.delete(id);
        await fetchJobs();
      } catch (err) {
        console.error("Failed to delete job:", err);
      }
    }
  };

  return (
    <div style={{ padding: '2rem' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
        <div>
          <h1 style={{ margin: 0 }}>Backup Jobs</h1>
          <p style={{ color: 'var(--secondary)', margin: 0 }}>Configure your SQL Server, MariaDB and File synchronization tasks.</p>
        </div>
        <ZestButton 
          zest={{
            visualOptions: {
              variant: 'success',
              iconLeft: <FaPlus />
            },
            semanticType: 'add'
          }}
          onClick={handleCreateNew}
        >
          Create New Job
        </ZestButton>
      </div>

      {error && (
        <div className="card" style={{ borderLeft: '4px solid var(--danger)', marginBottom: '2rem', backgroundColor: 'var(--danger-bg)' }}>
          <p style={{ color: 'var(--danger)', margin: 0 }}>{error}</p>
        </div>
      )}

      {isLoading ? (
        <div style={{ display: 'flex', justifyContent: 'center', padding: '4rem' }}>
          <p>Loading jobs...</p>
        </div>
      ) : (
        <JobsTable 
          jobs={jobs} 
          onTrigger={handleTrigger}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onShowHistory={handleShowHistory}
        />
      )}
    </div>
  );
}
