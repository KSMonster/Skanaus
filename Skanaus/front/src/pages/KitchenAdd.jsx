// KitchenAdd.jsx
import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const KitchenAdd = () => {
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    const handleAddKitchen = async () => {
        try {
            const accessToken = localStorage.getItem('accessToken');

            // Make sure name and description are not empty before sending the request
            if (name.trim() === '' || description.trim() === '') {
                setError('Name and description are required.');
                return;
            }

            await axios.post(
                'https://octopus-app-y47u2.ondigitalocean.app/api/kitchens',
                {
                    name: name,
                    description: description,
                },
                {
                    headers: {
                        Authorization: `Bearer ${accessToken}`,
                    },
                }
            );

            // After successful addition, navigate to the kitchens page or update the UI accordingly
            navigate('/kitchens');
        } catch (error) {
            setError(error.message);
        }
    };

    return (
        <div>
            <h1>Add Kitchen</h1>
            {error && <p>Error: {error}</p>}
            <label>
                Name:
                <input type="text" value={name} onChange={(e) => setName(e.target.value)} />
            </label>
            <br />
            <label>
                Description:
                <input type="text" value={description} onChange={(e) => setDescription(e.target.value)} />
            </label>
            <br />
            <button onClick={handleAddKitchen}>Add Kitchen</button>
        </div>
    );
};

export default KitchenAdd;
