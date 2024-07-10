import React from 'react';

interface InformationBannerProps {
    message: string;
}

const InformationBanner: React.FC<InformationBannerProps> = ({ message }) => {
    return <div className="information-banner">{message}</div>;
};

export default InformationBanner;
