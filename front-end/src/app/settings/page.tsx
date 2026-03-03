'use client';

import React, { useState } from 'react';
import ZestTextbox from 'jattac.libs.web.zest-textbox';
import ZestButton from 'jattac.libs.web.zest-button';
import { FaFloppyDisk, FaCloud, FaPlus, FaTrash } from 'react-icons/fa6';

interface CloudProvider {
    id: string;
    type: string;
    name: string;
    // Specific fields for different types
    s3AccessKey?: string;
    s3SecretKey?: string;
    s3Bucket?: string;
    azureConnectionString?: string;
    googleDriveFolderId?: string;
}

export default function SettingsPage() {
  const [providers, setProviders] = useState<CloudProvider[]>([
    { id: '1', type: 'S3', name: 'AWS Production', s3AccessKey: 'AKIA...', s3Bucket: 'prod-backups' }
  ]);
  const [defaultTarget, setDefaultTarget] = useState('D:\\Backups');

  const handleAddProvider = () => {
    setProviders([...providers, { id: Date.now().toString(), type: 'S3', name: 'New Provider' }]);
  };

  const updateProvider = (id: string, updates: Partial<CloudProvider>) => {
    setProviders(providers.map(p => p.id === id ? { ...p, ...updates } : p));
  };

  const handleSave = async () => {
    await new Promise(resolve => setTimeout(resolve, 800));
    alert("Settings synchronized successfully.");
  };

  const labelStyle: React.CSSProperties = { fontSize: '0.85rem', fontWeight: 600, color: 'var(--secondary)' };
  const inputGroupStyle: React.CSSProperties = { display: 'flex', flexDirection: 'column', gap: '0.4rem' };

  return (
    <div style={{ padding: '2rem' }}>
      <div style={{ marginBottom: '2rem' }}>
        <h1 style={{ margin: 0 }}>Settings</h1>
        <p style={{ color: 'var(--secondary)', margin: 0 }}>Configure your environment and cloud destinations with ease.</p>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '2rem' }}>
        <div className="card">
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
            <h2 style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', margin: 0 }}>
                <FaCloud style={{ color: 'var(--primary)' }} /> Cloud Destinations
            </h2>
            <ZestButton 
                zest={{ visualOptions: { variant: 'standard', size: 'sm' }, semanticType: 'add' }}
                onClick={handleAddProvider}
            >
                <FaPlus />
            </ZestButton>
          </div>
          
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
            {providers.map(p => (
                <div key={p.id} className="card" style={{ padding: '1rem', border: '1px solid var(--border)', position: 'relative' }}>
                    <div style={{ position: 'absolute', right: '0.5rem', top: '0.5rem' }}>
                        <ZestButton 
                            zest={{ visualOptions: { variant: 'danger', size: 'sm' }, semanticType: 'delete' }}
                            onClick={() => setProviders(providers.filter(item => item.id !== p.id))}
                        >
                            <FaTrash />
                        </ZestButton>
                    </div>

                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem', marginBottom: '1rem' }}>
                        <div style={inputGroupStyle}>
                            <label style={labelStyle}>Provider Type</label>
                            <select 
                                value={p.type}
                                style={{ padding: '0.5rem', borderRadius: '4px', border: '1px solid var(--border)', background: 'var(--card-bg)', color: 'var(--foreground)' }}
                                onChange={(e) => updateProvider(p.id, { type: e.target.value })}
                            >
                                <option value="S3">Amazon S3</option>
                                <option value="Azure">Azure Blob Storage</option>
                                <option value="GoogleDrive">Google Drive</option>
                            </select>
                        </div>
                        <div style={inputGroupStyle}>
                            <label style={labelStyle}>Friendly Name</label>
                            <ZestTextbox 
                                value={p.name}
                                zest={{ onTextChanged: (val) => updateProvider(p.id, { name: val || '' }), zSize: 'sm' }}
                            />
                        </div>
                    </div>

                    {/* DYNAMIC FIELDS PER PROVIDER */}
                    {p.type === 'S3' && (
                        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                            <div style={inputGroupStyle}>
                                <label style={labelStyle}>Access Key</label>
                                <ZestTextbox value={p.s3AccessKey || ''} zest={{ onTextChanged: (val) => updateProvider(p.id, { s3AccessKey: val }), zSize: 'sm' }} />
                            </div>
                            <div style={inputGroupStyle}>
                                <label style={labelStyle}>Bucket Name</label>
                                <ZestTextbox value={p.s3Bucket || ''} zest={{ onTextChanged: (val) => updateProvider(p.id, { s3Bucket: val }), zSize: 'sm' }} />
                            </div>
                        </div>
                    )}

                    {p.type === 'Azure' && (
                        <div style={inputGroupStyle}>
                            <label style={labelStyle}>Connection String</label>
                            <ZestTextbox value={p.azureConnectionString || ''} zest={{ onTextChanged: (val) => updateProvider(p.id, { azureConnectionString: val }), zSize: 'sm' }} />
                        </div>
                    )}

                    {p.type === 'GoogleDrive' && (
                        <div style={inputGroupStyle}>
                            <label style={labelStyle}>Folder ID</label>
                            <ZestTextbox value={p.googleDriveFolderId || ''} zest={{ onTextChanged: (val) => updateProvider(p.id, { googleDriveFolderId: val }), zSize: 'sm' }} />
                        </div>
                    )}
                </div>
            ))}
          </div>
        </div>

        <div className="card">
          <h2>App Preferences</h2>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem', marginTop: '1.5rem' }}>
            <div style={inputGroupStyle}>
                <label style={labelStyle}>Default Local Backup Path</label>
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
          Commit Settings
        </ZestButton>
      </div>
    </div>
  );
}
