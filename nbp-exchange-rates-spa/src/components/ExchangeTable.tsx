import React, { useEffect, useState } from 'react';
import TableCell from './TableCell';
import InformationBanner from './InformationBanner';

interface ExchangeRateDto {
    currencyCode: string;
    currencyName: string;
    rate: number;
}

interface ExchangeRatesTableDto {
    exchangeRates: ExchangeRateDto[];
    nbpTableId: string;
    effectiveDate: string;
    savedAt: string;
    isFromDatabase: boolean;
    message?: string;
}

interface ExchangeTableProps {
    url: string;
}

const ExchangeTable: React.FC<ExchangeTableProps> = ({ url }) => {
    const [exchangeRatesTableDto, setExchangeRatesTableDto] = useState<ExchangeRatesTableDto | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        fetch(url)
            .then(response => {
                if (response.status === 404) {
                    throw new Error('Nie znaleziono kursów ani w bazie ani z API NBP.');
                }
                return response.json();
            })
            .then(data => {
                setExchangeRatesTableDto(data);
                setLoading(false);
            })
            .catch(error => {
                if (error.message === 'Failed to fetch') {
                    setError('Nie można połączyć się z serwerem. Sprawdź połączenie internetowe i spróbuj ponownie.');
                } else {
                    setError(error.message);
                }
                setLoading(false);
            });
    }, [url]);

    const formatDate = (dateString: string) => {
        const date = new Date(dateString);
        if (isNaN(date.getTime())) {
            return 'Nieprawidłowa data';
        }
        return new Intl.DateTimeFormat('pl-PL', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        }).format(date);
    };

    if (loading) {
        return <div>Ładowanie...</div>;
    }

    if (error) {
        return <InformationBanner message={error} />;
    }

    if (!exchangeRatesTableDto || !exchangeRatesTableDto.exchangeRates.length) {
        return <InformationBanner message="Brak dostępnych danych do wyświetlenia. Spróbuj ponownie później." />;
    }

    return (
        <div>
            {exchangeRatesTableDto.isFromDatabase && exchangeRatesTableDto.message && (
                <InformationBanner message={exchangeRatesTableDto.message} />
            )}
            <div>
                <p><strong>Numer tabeli NBP:</strong> {exchangeRatesTableDto.nbpTableId}</p>
                <p><strong>Data wejścia w życie:</strong> {formatDate(exchangeRatesTableDto.effectiveDate)}</p>
                <p><strong>Data pobrania danych:</strong> {formatDate(exchangeRatesTableDto.savedAt)}</p>
            </div>
            <table>
                <thead>
                <tr>
                    <th>Kod waluty</th>
                    <th>Nazwa waluty</th>
                    <th>Kurs średni PLN</th>
                </tr>
                </thead>
                <tbody>
                {exchangeRatesTableDto.exchangeRates.map((rate, index) => (
                    <tr key={index}>
                        <TableCell>{rate.currencyCode}</TableCell>
                        <TableCell>{rate.currencyName}</TableCell>
                        <TableCell>{rate.rate}</TableCell>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
};

export default ExchangeTable;
