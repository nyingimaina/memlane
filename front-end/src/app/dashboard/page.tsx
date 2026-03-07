'use client';

import React from 'react';
import { useJobLogic } from '@/logic/useJobLogic';
import JobsTable from '@/components/JobsTable';
import { useUI } from '@/logic/UIContext';
import JobForm from '@/features/JobManagement/UI/JobForm';
import TutorialIcon from '@/components/TutorialIcon';
import JobHistoryList from '@/components/JobHistoryList';
import RunLogViewer from '@/components/RunLogViewer';
import { JobMetadata, JobStatus, JobRun } from '@/models/Job';
import { JobRepository } from '@/repositories/JobRepository';
import { FaDatabase, FaCircleCheck, FaTriangleExclamation, FaHardDrive } from 'react-icons/fa6';

export default function DashboardPage() {
  const { jobs, isLoading, error, fetchJobs } = useJobLogic();
  const { openSidePane, closeSidePane, triggerTutorial } = useUI();

  const handleTrigger = async (id: number) => {
    try {
        await JobRepository.trigger(id);
        await fetchJobs();
    } catch (err) {
        console.error("Failed to trigger job:", err);
        throw err; // Re-throw for ZestButton feedback
    }
  };

  const activeJobs = jobs.filter(j => j.status === JobStatus.InProgress).length;
  const successCount = jobs.filter(j => j.status === JobStatus.Completed).length;
  const failCount = jobs.filter(j => j.status === JobStatus.Failed).length;

  const handleEdit = (job: JobMetadata) => {
    openSidePane(
      <JobForm 
        initialJob={job}
        onSubmit={async (data) => {
            await JobRepository.update(job.id, data);
            closeSidePane();
            await fetchJobs();
        }}
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
    if (confirm("Delete this job?")) {
        await JobRepository.delete(id);
        await fetchJobs();
    }
  };

  return (
    <div style={{ padding: '2rem' }}>
      <div style={{ marginBottom: '2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }} className="dashboard-header">
        <div>
            <h1 style={{ margin: 0 }}>Dashboard</h1>
            <p style={{ color: 'var(--secondary)', margin: 0 }}>Real-time overview of your backup infrastructure.</p>
        </div>
        <TutorialIcon onClick={triggerTutorial} />
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '1.5rem', marginBottom: '3rem' }} className="jobs-summary-card">
        <SummaryCard title="Total Jobs" value={jobs.length.toString()} icon={<FaDatabase />} color="var(--primary)" />
        <SummaryCard title="Active Now" value={activeJobs.toString()} icon={<FaHardDrive />} color="var(--info)" isPulse={activeJobs > 0} />
        <SummaryCard title="Success Rate" value={jobs.length > 0 ? `${Math.round((successCount / (successCount + failCount || 1)) * 100)}%` : '100%'} icon={<FaCircleCheck />} color="var(--success)" />
        <SummaryCard title="Recent Failures" value={failCount.toString()} icon={<FaTriangleExclamation />} color="var(--danger)" />
      </div>

      <div style={{ marginBottom: '1.5rem' }}>
        <h2>Live Monitoring</h2>
      </div>

      {isLoading ? (
        <div style={{ display: 'flex', justifyContent: 'center', padding: '4rem' }}>
          <p>Analyzing pipelines...</p>
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

const SummaryCard = ({ title, value, icon, color, isPulse }: { title: string, value: string, icon: React.ReactNode, color: string, isPulse?: boolean }) => (
    <div className={`card ${isPulse ? 'status-active' : ''}`} style={{ display: 'flex', alignItems: 'center', gap: '1.5rem' }}>
        <div style={{ fontSize: '2rem', color }}>{icon}</div>
        <div>
            <div style={{ color: 'var(--secondary)', fontSize: '0.85rem', fontWeight: 600 }}>{title}</div>
            <div style={{ fontSize: '1.5rem', fontWeight: 700 }}>{value}</div>
        </div>
    </div>
);
