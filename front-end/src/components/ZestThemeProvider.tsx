'use client';

import React from 'react';
import ZestButtonConfigProvider from 'jattac.libs.web.zest-button/dist/ZestButtonConfigProvider';
import { ZestTextboxConfigProvider } from 'jattac.libs.web.zest-textbox/dist/contexts/ZestTextboxConfigContext';

interface ZestThemeProviderProps {
    children: React.ReactNode;
}

const ZestThemeProvider: React.FC<ZestThemeProviderProps> = ({ children }) => {
    // Joy-inducing defaults for Buttons
    const buttonConfig = {
        defaultProps: {
            theme: 'system' as const,
            busyOptions: {
                handleInternally: true,
                preventRageClick: true,
                minBusyDurationMs: 600 // Slight delay for satisfying feedback
            },
            successOptions: {
                showCheckmark: true,
                autoResetAfterMs: 2000
            }
        }
    };

    // Joy-inducing defaults for Textboxes
    const textboxConfig = {
        theme: 'system' as const,
        animatedCounter: true,
        showProgressBar: true,
        helperTextPositioning: 'reserved' as const,
        zSize: 'md' as const
    };

    return (
        <ZestButtonConfigProvider config={buttonConfig}>
            <ZestTextboxConfigProvider value={textboxConfig}>
                {children}
            </ZestTextboxConfigProvider>
        </ZestButtonConfigProvider>
    );
};

export default ZestThemeProvider;
