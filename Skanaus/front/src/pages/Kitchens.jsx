// Kitchens.jsx
import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Link, useNavigate } from 'react-router-dom';

const Kitchens = () => {
    const [kitchens, setKitchens] = useState([]);
    const [error, setError] = useState(null);
    const [pageNumber, setPageNumber] = useState(1);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchData = async () => {
            try {
                const result = await axios.get(`https://octopus-app-y47u2.ondigitalocean.app/api/kitchens?pageNumber=${pageNumber}`);
                setKitchens(result.data);
            } catch (error) {
                setError(error.message);
            }
        };

        fetchData();
    }, [pageNumber]);

    const handleDelete = async (id) => {
        const confirmDelete = window.confirm('Are you sure you want to delete this kitchen?');

        if (confirmDelete) {
            try {
                const accessToken = localStorage.getItem('accessToken');

                await axios.delete(`https://octopus-app-y47u2.ondigitalocean.app/api/kitchens/${id}`, {
                    headers: {
                        Authorization: `Bearer ${accessToken}`,
                    },
                });

                // After successful deletion, you may want to refresh the kitchen list or update the UI accordingly.
                // For simplicity, refetching the data in this example
                const result = await axios.get(`https://octopus-app-y47u2.ondigitalocean.app/api/kitchens?pageNumber=${pageNumber}`);
                setKitchens(result.data);

                // Check if the current page is empty, and navigate to the previous page if so
                if (result.data.length === 0 && pageNumber > 1) {
                    setPageNumber((prevPage) => prevPage - 1);
                }
            } catch (error) {
                setError(error.message);
            }
        }
    };
    
    const handleNextPage = async () => {
        try {
            // Fetch data for the next page
            const nextPageResult = await axios.get(`https://octopus-app-y47u2.ondigitalocean.app/api/kitchens?pageNumber=${pageNumber + 1}`);

            // Update the page number only if there is data on the next page
            if (nextPageResult.data.length > 0) {
                setPageNumber((prevPage) => prevPage + 1);
            }
        } catch (error) {
            setError(error.message);
        }
    };

    const handlePrevPage = () => {
        if (pageNumber > 1) {
            setPageNumber((prevPage) => prevPage - 1);
        }
    };

    return (
        <div>
            <h1>Kitchens</h1>
            {error && <p>Error: {error}</p>}
            <ul>
                {kitchens.map((kitchen) => (
                    <li key={kitchen.id}>
                        <strong>Name:</strong> {kitchen.name} <br />
                        <strong>Description:</strong> {kitchen.description} <br />
                        <strong>Creation Date:</strong> {new Date(kitchen.creationDate).toLocaleString()} <br />

                        <Link to={`/kitchen/${kitchen.id}`}>
                            <button>Open Kitchen</button>
                        </Link>

                        <Link to={`/kitchenupdate/${kitchen.id}`}>
                            <button>Update Kitchen</button>
                        </Link>

                        <button onClick={() => handleDelete(kitchen.id)}>Delete Kitchen</button>

                        <br />
                        <br />
                    </li>
                ))}
            </ul>

            <div>
                <button onClick={handlePrevPage} disabled={pageNumber === 1}>
                    Previous Page
                </button>

                <button onClick={handleNextPage}>Next Page</button>
            </div>

            <button onClick={() => navigate('/kitchenadd')}>Add Kitchen</button>
        </div>
    );
};

export default Kitchens;
