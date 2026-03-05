/* eslint-disable @typescript-eslint/no-explicit-any */
import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import JobForm from './JobForm';
import { UIProvider } from '@/logic/UIContext';

// Mock Zest components
jest.mock('jattac.libs.web.zest-textbox', () => {
    const MockZestTextbox = ({ value, zest, placeholder, type }: { value: string, zest: any, placeholder: string, type?: string }) => (
        <input 
            data-testid={`zest-textbox-${placeholder}`}
            value={value} 
            type={type || 'text'}
            onChange={(e) => zest?.onTextChanged?.(e.target.value)} 
            placeholder={placeholder}
        />
    );
    MockZestTextbox.displayName = 'MockZestTextbox';
    return MockZestTextbox;
});

jest.mock('jattac.libs.web.zest-button', () => {
    const MockZestButton = ({ children, onClick, zest }: { children: React.ReactNode, onClick: () => void, zest: any }) => (
        <button onClick={onClick} data-testid={`zest-button-${zest?.semanticType || 'default'}`}>
            {children}
        </button>
    );
    MockZestButton.displayName = 'MockZestButton';
    return MockZestButton;
});

describe('JobForm', () => {
    const mockOnSubmit = jest.fn();
    const mockOnCancel = jest.fn();

    const renderWithContext = (ui: React.ReactElement) => {
        return render(
            <UIProvider>
                {ui}
            </UIProvider>
        );
    };

    beforeEach(() => {
        mockOnSubmit.mockClear();
        mockOnCancel.mockClear();
    });

    it('renders with initial values', () => {
        renderWithContext(<JobForm onSubmit={mockOnSubmit} onCancel={mockOnCancel} />);
        
        expect(screen.getByText('Pipeline Name')).toBeInTheDocument();
        expect(screen.getByPlaceholderText('e.g., Production SQL Server')).toBeInTheDocument();
        expect(screen.getByText('Automation & Retention')).toBeInTheDocument();
    });

    it('updates job name and retention count', async () => {
        renderWithContext(<JobForm onSubmit={mockOnSubmit} onCancel={mockOnCancel} />);
        
        const nameInput = screen.getByPlaceholderText('e.g., Production SQL Server');
        const retentionInput = screen.getByDisplayValue('5');

        fireEvent.change(nameInput, { target: { value: 'Test Job' } });
        fireEvent.change(retentionInput, { target: { value: '10' } });

        const submitButton = screen.getByTestId('zest-button-save');
        fireEvent.click(submitButton);

        expect(mockOnSubmit).toHaveBeenCalledWith(expect.objectContaining({
            name: 'Test Job',
            configurationJson: expect.stringContaining('"retentionCount":10')
        }));
    });

    it('updates cron schedule via CronBuilder', async () => {
        renderWithContext(<JobForm onSubmit={mockOnSubmit} onCancel={mockOnCancel} />);
        
        // Find the cron select (it has 'Manual Only' as default)
        const cronSelect = screen.getByDisplayValue('Manual Only');
        fireEvent.change(cronSelect, { target: { value: '0 0 * * *' } });

        const submitButton = screen.getByTestId('zest-button-save');
        fireEvent.click(submitButton);

        expect(mockOnSubmit).toHaveBeenCalledWith(expect.objectContaining({
            cronExpression: '0 0 * * *'
        }));
    });
});
