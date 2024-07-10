import React from 'react';
import './App.css';
import ExchangeTable from "./components/ExchangeTable";
import DataInfo from "./components/DataInfo";

const App: React.FC = () => {
    return (
        <div className="app-container">
            <header>
                <h1>Kursy wymiany walut Narodowego Banku Polskiego</h1>
            </header>
            <main>
                <ExchangeTable url="http://localhost:5234/CurrencyRates"/>
                <section className="info-section">
                    <h2>Informacje o danych</h2>
                    <DataInfo />
                </section>
            </main>
        </div>
    );
};

export default App;
