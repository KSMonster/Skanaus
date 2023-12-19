import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';

const Kitchen = () => {
    const { id } = useParams();
    const [kitchen, setKitchen] = useState(null);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchKitchen = async () => {
            try {
                const result = await axios.get(`https://octopus-app-y47u2.ondigitalocean.app/api/kitchens/${id}`);
                setKitchen(result.data);
            } catch (error) {
                setError(error.message);
            }
        };

        fetchKitchen();
    }, [id]);

    return (
        <div>
            <h2>Kitchen</h2>
            {error && <p>Error: {error}</p>}
            {kitchen && (
                <div>
                    <p><strong>Name:</strong> {kitchen.name}</p>
                    <p><strong>Description:</strong> {kitchen.description}</p>
                    <p><strong>Creation Date:</strong> {new Date(kitchen.creationDate).toLocaleString()}</p>
                </div>
            )}
        </div>
    );
};

export default Kitchen;
