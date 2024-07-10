import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import ExchangeTable from '../components/ExchangeTable';
import { http } from 'msw';
import { setupServer } from 'msw/node';

const server = setupServer(
    http.get('http://localhost:5234/CurrencyRates', () => {
        return new Response(JSON.stringify({
            exchangeRates: [
                { currencyCode: 'USD', currencyName: 'dolar amerykański', rate: 3.8956 },
                { currencyCode: 'EUR', currencyName: 'euro', rate: 4.2157 },
            ],
            nbpTableId: '133/A/NBP/2024',
            effectiveDate: '2024-07-10T00:00:00',
            savedAt: '2024-07-10T08:30:54.8241023Z',
            isFromDatabase: false,
            message: '',
        }), {
            headers: { 'Content-Type': 'application/json' },
        });
    })
);

beforeAll(() => server.listen());
afterEach(() => server.resetHandlers());
afterAll(() => server.close());

test('renders loading state initially', () => {
    render(<ExchangeTable url="http://localhost:5234/CurrencyRates" />);
    expect(screen.getByText(/Ładowanie.../i)).toBeInTheDocument();
});

test('renders exchange rates after loading', async () => {
    render(<ExchangeTable url="http://localhost:5234/CurrencyRates" />);

    await waitFor(() => expect(screen.getByText('133/A/NBP/2024')).toBeInTheDocument());
    expect(screen.getByText('10.07.2024, 00:00:00')).toBeInTheDocument();
    expect(screen.getByText('10.07.2024, 08:30:54')).toBeInTheDocument();
    expect(screen.getByText('USD')).toBeInTheDocument();
    expect(screen.getByText('dolar amerykański')).toBeInTheDocument();
    expect(screen.getByText('3.8956')).toBeInTheDocument();
    expect(screen.getByText('EUR')).toBeInTheDocument();
    expect(screen.getByText('euro')).toBeInTheDocument();
    expect(screen.getByText('4.2157')).toBeInTheDocument();
});


