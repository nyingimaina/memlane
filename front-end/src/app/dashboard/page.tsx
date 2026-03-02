'use client';

import React, { useState } from 'react';
import ZestButton from 'jattac.libs.web.zest-button';
import ZestTextbox from 'jattac.libs.web.zest-textbox';

export default function DashboardPage() {
  const [testValue, setTestValue] = useState('');

  const handleAsyncAction = async () => {
    // Simulate a delightful async backup trigger
    await new Promise(resolve => setTimeout(resolve, 1500));
    console.log("Action completed!");
  };

  return (
    <div className="card" style={{ maxWidth: '600px', margin: '2rem auto' }}>
      <h1>Dashboard Overview</h1>
      <p style={{ marginBottom: '2rem' }}>Welcome to your Memlane control center. Experience the joy of reliable backups.</p>
      
      <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
        <ZestTextbox 
          placeholder="Enter a job name to test feedback..."
          maxLength={50}
          value={testValue}
          onChange={(e) => setTestValue(e.target.value)}
          zest={{
            helperTextConfig: {
              formatter: () => testValue ? `You are naming this: ${testValue}` : "Give your job a unique name"
            }
          }}
        />

        <ZestButton 
          onClick={handleAsyncAction}
          zest={{
            visualOptions: {
              variant: 'success',
              stretch: true
            },
            semanticType: 'add'
          }}
        >
          Trigger Test Job
        </ZestButton>
      </div>
    </div>
  );
}
