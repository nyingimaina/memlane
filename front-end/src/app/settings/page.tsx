'use client';

import React, { useState } from 'react';
import ZestTextbox from 'jattac.libs.web.zest-textbox';
import ZestButton from 'jattac.libs.web.zest-button';
import { FaFloppyDisk, FaCloud } from 'react-icons/fa6';

export default function SettingsPage() {
  const [s3Key, setS3Key] = useState('');
  const [s3Secret, setS3Secret] = useState('');
  const [defaultTarget, setDefaultTarget] = useState('D:\\Backups');

  const handleSave = async () => {
    // Simulate saving settings
    await new Promise(resolve => setTimeout(resolve, 800));
    alert("Settings saved successfully!");
  };

  return (
    <div style={{ padding: '2rem' }}>
      <div style={{ marginBottom: '2rem' }}>
        <h1 style={{ margin: 0 }}>Settings</h1>
        <p style={{ color: 'var(--secondary)', margin: 0 }}>Configure global preferences and cloud provider credentials.</p>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '2rem' }}>
        <div className="card">
          <h2 style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginBottom: '1.5rem' }}>
            <FaCloud style={{ color: 'var(--primary)' }} /> Cloud Credentials
          </h2>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                <label style={{ fontWeight: 600, fontSize: '0.9rem' }}>S3 Access Key</label>
                <ZestTextbox 
                    placeholder="AKIA..."
                    value={s3Key}
                    zest={{ onTextChanged: (val) => setS3Key(val || '') }}
                />
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                <label style={{ fontWeight: 600, fontSize: '0.9rem' }}>S3 Secret Key</label>
                <ZestTextbox 
                    type="password"
                    placeholder="••••••••••••••••"
                    value={s3Secret}
                    zest={{ onTextChanged: (val) => setS3Secret(val || '') }}
                />
            </div>
          </div>
        </div>

        <div className="card">
          <h2>Application Defaults</h2>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem', marginTop: '1.5rem' }}>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                <label style={{ fontWeight: 600, fontSize: '0.9rem' }}>Default Target Directory</label>
                <ZestTextbox 
                    placeholder="D:\Backups"
                    value={defaultTarget}
                    zest={{ onTextChanged: (val) => setDefaultTarget(val || '') }}
                />
            </div>
          </div>
        </div>
      </div>

      <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '2rem' }}>
        <ZestButton 
          onClick={handleSave}
          zest={{
            visualOptions: {
              variant: 'success',
              iconLeft: <FaFloppyDisk />
            },
            semanticType: 'save'
          }}
        >
          Save All Settings
        </ZestButton>
      </div>
    </div>
  );
}
