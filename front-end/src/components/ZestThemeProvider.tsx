'use client';

import React from 'react';
import { ZestButtonConfigProvider } from 'jattac.libs.web.zest-button';
import { ZestTextboxConfigProvider } from 'jattac.libs.web.zest-textbox';

interface ZestThemeProviderProps {
    children: React.ReactNode;
}

const ZestThemeProvider: React.FC<ZestThemeProviderProps> = ({ children }) => {
    // Joy-inducing defaults for Buttons
    const buttonConfig = {
        defaultProps: {
            theme: 'system' as const,
            visualOptions: {
                stretch: false
            },
            busyOptions: {
                handleInternally: true,
                preventRageClick: true,
                minBusyDurationMs: 600
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
        zSize: 'md' as const,
        stretch: true
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
