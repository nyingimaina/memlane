/* eslint-disable @typescript-eslint/no-explicit-any */
import React from 'react';
import { render, screen } from '@testing-library/react';
import JobHealthIcon from './JobHealthIcon';

describe('JobHealthIcon', () => {
    it('renders Sunny icon for high scores (>= 80)', () => {
        render(<JobHealthIcon score={100} total={5} success={5} />);
        const container = screen.getByTitle(/Sunny: Very stable \(5\/5 recent runs successful\)/);
        expect(container).toBeInTheDocument();
    });

    it('renders Partly Cloudy icon for moderate-high scores (60-79)', () => {
        render(<JobHealthIcon score={75} total={4} success={3} />);
        const container = screen.getByTitle(/Partly Cloudy: Stable \(3\/4 recent runs successful\)/);
        expect(container).toBeInTheDocument();
    });

    it('renders Cloudy icon for moderate scores (40-59)', () => {
        render(<JobHealthIcon score={50} total={2} success={1} />);
        const container = screen.getByTitle(/Cloudy: Unstable \(1\/2 recent runs successful\)/);
        expect(container).toBeInTheDocument();
    });

    it('renders Rainy icon for low scores (20-39)', () => {
        render(<JobHealthIcon score={30} total={5} success={1} />);
        const container = screen.getByTitle(/Rainy: Often fails \(1\/5 recent runs successful\)/);
        expect(container).toBeInTheDocument();
    });

    it('renders Stormy icon for very low scores (< 20)', () => {
        render(<JobHealthIcon score={10} total={5} success={0} />);
        const container = screen.getByTitle(/Stormy: Critical failures \(0\/5 recent runs successful\)/);
        expect(container).toBeInTheDocument();
    });

    it('renders "No runs yet" when total is 0', () => {
        render(<JobHealthIcon score={100} total={0} success={0} />);
        const container = screen.getByTitle(/No runs yet/);
        expect(container).toBeInTheDocument();
    });
});
