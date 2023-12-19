// KitchenUpdate.jsx
import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams, useNavigate } from 'react-router-dom';

const KitchenUpdate = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [kitchen, setKitchen] = useState({ name: '', description: '' });
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchKitchenData = async () => {
            try {
                const result = await axios.get(`https://octopus-app-y47u2.ondigitalocean.app/api/kitchens/${id}`);
                setKitchen(result.data);
            } catch (error) {
                setError(error.message);
            }
        };

        fetchKitchenData();
    }, [id]);

    const handleUpdate = async () => {
        try {
            const accessToken = localStorage.getItem('accessToken');

            await axios.put(
                `https://octopus-app-y47u2.ondigitalocean.app/api/kitchens/${id}`,
                {
                    description: kitchen.description,
                },
                {
                    headers: {
                        Authorization: `Bearer ${accessToken}`,
                    },
                }
            );

            // Redirect to /kitchens after the update
            navigate('/kitchens');
        } catch (error) {
            setError(error.message);
        }
    };

    return (
        <div>
            <h1>Update Kitchen</h1>
            {error && <p>Error: {error}</p>}
            <p>
                <strong>Name:</strong> {kitchen.name}
            </p>
            <label>
                Description:
                <input type="text" value={kitchen.description} onChange={(e) => setKitchen({ ...kitchen, description: e.target.value })} />
            </label>
            <br />
            <button onClick={handleUpdate}>Update</button>
        </div>
    );
};

export default KitchenUpdate;
