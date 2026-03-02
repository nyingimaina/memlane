import axios from 'axios';
import { JobMetadata, JobStatus } from '@/models/Job';

const API_URL = process.env.NEXT_PUBLIC_API_URL;

export const JobRepository = {
    getAll: async (): Promise<JobMetadata[]> => {
        const response = await axios.get(`${API_URL}/jobs`);
        return response.data;
    },

    getById: async (id: number): Promise<JobMetadata> => {
        const response = await axios.get(`${API_URL}/jobs/${id}`);
        return response.data;
    },

    create: async (job: Partial<JobMetadata>): Promise<JobMetadata> => {
        const response = await axios.post(`${API_URL}/jobs`, job);
        return response.data;
    },

    update: async (id: number, job: Partial<JobMetadata>): Promise<JobMetadata> => {
        const response = await axios.put(`${API_URL}/jobs/${id}`, job);
        return response.data;
    },

    delete: async (id: number): Promise<void> => {
        await axios.delete(`${API_URL}/jobs/${id}`);
    },

    trigger: async (id: number): Promise<void> => {
        await axios.post(`${API_URL}/jobs/${id}/trigger`);
    }
};
