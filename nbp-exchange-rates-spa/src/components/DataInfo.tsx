import React from 'react';
import '../App.css';

const DataInfo: React.FC = () => {
    return (
        <div className="data-info-container">
            <p>Dane pobierane są z serwisu Narodowego Banku Polskiego. Informacje podzielone są na tak zwane tabele, z czego każda jest określona inną literą. Wyróżnia się:</p>
            <ul>
                <li><strong>tabela A</strong> (aktualna) – zawiera wartości 34 najpopularniejszych walut włącznie z polskim złotym, dane aktualizowane są codziennie, zazwyczaj około godziny 16</li>
                <li><strong>tabela B</strong> (niedostępna obecnie w aplikacji) – można w niej odszukać nieco rzadziej spotykane waluty, warto również pamiętać, że tabela B nie jest aktualizowana codziennie, a zmiana danych odbywa się zaledwie raz w tygodniu – dokładniej w środę pomiędzy godziną 11:45 a 12:15</li>
                <li><strong>tabela C</strong> (niedostępna obecnie w aplikacji) – inaczej nazywana jest archiwum, można znaleźć w niej prowadzone przez Narodowy Bank Polski zaległe notowania kursów walut zagranicznych, które wykorzystuje się do prowadzenia statystyk oraz monitorowania zmian sytuacji na rynku walut</li>
            </ul>
        </div>
    );
}

export default DataInfo;
