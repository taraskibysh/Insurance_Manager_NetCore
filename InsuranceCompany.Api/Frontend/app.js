document.addEventListener('DOMContentLoaded', () => {
    const registerForm = document.getElementById('register-form');
    const loginForm = document.getElementById('login-form');
    const createInsuranceForm = document.getElementById('create-insurance-form');
    const authSection = document.getElementById('auth-section');
    const insuranceSection = document.getElementById('insurance-section');
    const insuranceList = document.getElementById('insurance-list');
    const backToAuthBtn = document.getElementById('back-to-auth');

    const typeSelect = document.getElementById('type');
    const methodSelect = document.getElementById('method');
    const statusSelect = document.getElementById('status');

    let authToken = localStorage.getItem('authToken') || '';

    // Values for select dropdowns
    const insuranceEnums = {
        TypeOfInsurance: [
            { value: 'CarInsurance', text: 'Car Insurance' },
            { value: 'HealthInsurance', text: 'Health Insurance' },
            { value: 'HouseInsurance', text: 'House Insurance' },
            { value: 'AnotherInsurance', text: 'Another Insurance' },
        ],
        MethodOfInsurance: [
            { value: 'FullInsurance', text: 'Full Insurance' },
            { value: 'HalfInsurance', text: 'Half Insurance' },
            { value: 'QuarterInsurance', text: 'Quarter Insurance' },
        ],
        InsuranceStatus: [
            { value: 'Active', text: 'Active' },
            { value: 'Canceled', text: 'Canceled' },
        ],
    };

    // Populate select dropdowns
    function populateSelect(selectElement, options) {
        options.forEach(option => {
            const opt = document.createElement('option');
            opt.value = option.value;
            opt.textContent = option.text;
            selectElement.appendChild(opt);
        });
    }

    populateSelect(typeSelect, insuranceEnums.TypeOfInsurance);
    populateSelect(methodSelect, insuranceEnums.MethodOfInsurance);
    populateSelect(statusSelect, insuranceEnums.InsuranceStatus);

    // Check if user is already logged in
    if (authToken) {
        authSection.style.display = 'none';
        insuranceSection.style.display = 'block';
        fetchAllInsurances();
    }

    // Function to fetch all insurances
    async function fetchAllInsurances() {
        try {
            const response = await fetch('https://localhost:7279/controller/all', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${authToken}`,
                },
            });

            if (response.ok) {
                const insurances = await response.json();
                renderInsuranceList(insurances);
            } else {
                alert('Failed to fetch insurances.');
            }
        } catch (error) {
            console.error('Error fetching insurances:', error);
        }
    }

    // Function to render insurance list
    function renderInsuranceList(insurances) {
        insuranceList.innerHTML = '';
        if (insurances.length === 0) {
            insuranceList.innerHTML = '<p>No insurances available.</p>';
            return;
        }

        insurances.forEach((insurance) => {
            const item = document.createElement('div');
            item.className = 'insurance-item';
            item.innerHTML = `
                <link rel="stylesheet" href="styles.css">
                <p><strong>Type:</strong> ${insurance.typeOfInsurance}</p>
                <p><strong>Status:</strong> ${insurance.status}</p>
                <p><strong>Price:</strong> ${insurance.price}</p>
                <hr style="border: 5px solid green; width: 100%; margin: 10px auto;">
            `;

            // Add event listener to open new tab with insurance details
            item.addEventListener('click', () => openInsuranceDetails(insurance.id));

            item.addEventListener('mouseover', () => {
                item.style.cursor = 'pointer'; // Змінюємо курсор на "руку" при наведенні
            });
            
            item.addEventListener('mouseout', () => {
                item.style.cursor = 'default'; // Відновлюємо стандартний курсор, коли мишка покидає елемент
            });
            
            insuranceList.appendChild(item);
        });
    }

    // Function to open new tab with insurance details
    async function openInsuranceDetails(insuranceId) {
        try {
            const response = await fetch(`https://localhost:7279/controller/Id?id=${insuranceId}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${authToken}`,
                }
            });
    
            if (!response.ok) {
                throw new Error('Failed to fetch insurance details');
            }
    
            const insuranceDetails = await response.json();
            // Pass only the insuranceId in the URL, not the rendered details
            const url = `insurance_details.html?id=${insuranceId}`;
            window.location.href = url;
        } catch (error) {
            console.error('Error opening insurance details:', error);
            alert('Could not fetch insurance details.');
        }
    }
    

    // User registration
    registerForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const firstName = document.getElementById('firstName').value;
        const lastName = document.getElementById('lastName').value;
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;

        try {
            const response = await fetch('https://localhost:7279/auth/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ firstName, lastName, email, password }),
            });

            if (response.ok) {
                alert('Registration successful! Please log in.');
            } else {
                const data = await response.json();
                alert(data.message || 'Registration failed.');
            }
        } catch (error) {
            alert('Error: ' + error.message);
        }
    });

    document.addEventListener('DOMContentLoaded', () => {
        const editBtn = document.getElementById('edit-btn');
        const deleteBtn = document.getElementById('delete-btn');
        const backBtn = document.getElementById('back-btn');
        const editForm = document.getElementById('edit-form');
        const insuranceInfo = document.getElementById('insurance-info');
        const insuranceId = new URLSearchParams(window.location.search).get('id');
    
        let authToken = localStorage.getItem('authToken') || '';
    
        // Fetch insurance details and populate edit form
        async function fetchInsuranceDetails() {
            try {
                const response = await fetch(`https://localhost:7279/controller/Id?id=${insuranceId}`, {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${authToken}`,
                    }
                });
    
                if (!response.ok) {
                    throw new Error('Failed to fetch insurance details');
                }
    
                const insurance = await response.json();
                renderInsuranceDetails(insurance);
            } catch (error) {
                console.error('Error fetching insurance details:', error);
                alert('Could not fetch insurance details.');
            }
        }
    
        // Render insurance details
        function renderInsuranceDetails(insurance) {
            insuranceInfo.innerHTML = `
                <p><strong>ID:</strong> ${insurance.id}</p>
                <p><strong>Type:</strong> ${insurance.typeOfInsurance}</p>
                <p><strong>Method:</strong> ${insurance.methodOfInsurance}</p>
                <p><strong>Status:</strong> ${insurance.status}</p>
                <p><strong>Price:</strong> ${insurance.price}</p>
                <p><strong>Price:</strong> ${insurance.pay}</p>
                <p><strong>Price:</strong> ${insurance.userId}</p>
            `;
    
            // Populate edit form fields
            document.getElementById('edit-type').value = insurance.typeOfInsurance;
            document.getElementById('edit-method').value = insurance.methodOfInsurance;
            document.getElementById('edit-status').value = insurance.status;
            document.getElementById('edit-price').value = insurance.price;
            document.getElementById('edit-pay').value = insurance.pay;

        }
    
        // Edit insurance functionality
        editBtn.addEventListener('click', () => {
            editForm.style.display = 'block';
            insuranceInfo.style.display = 'none';
        });
    
        // Save edited insurance
        editForm.addEventListener('submit', async (e) => {
            e.preventDefault();
    
            const type = document.getElementById('edit-type').value;
            const method = document.getElementById('edit-method').value;
            const status = document.getElementById('edit-status').value;
            let price = document.getElementById('edit-price').value;
    
            price = parseFloat(price);
            if (isNaN(price)) {
                alert('Please enter a valid price.');
                return;
            }
    
            try {
                const response = await fetch(`https://localhost:7279/controller/Id?id=${insuranceId}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${authToken}`,
                    },
                    body: JSON.stringify({ type, method, status, price })
                });
    
                if (response.ok) {
                    alert('Insurance updated successfully!');
                    fetchInsuranceDetails();
                } else {
                    const data = await response.json();
                    alert(data.message || 'Failed to update insurance.');
                }
            } catch (error) {
                alert('Error: ' + error.message);
            }
        });
    
        // Delete insurance
        deleteBtn.addEventListener('click', async () => {
            const confirmDelete = confirm('Are you sure you want to delete this insurance?');
            if (confirmDelete) {
                try {
                    const response = await fetch(`https://localhost:7279/controller/Id?id=${insuranceId}`, {
                        method: 'DELETE',
                        headers: {
                            'Authorization': `Bearer ${authToken}`,
                        }
                    });
    
                    if (response.ok) {
                        alert('Insurance deleted successfully!');
                        window.location.href = 'index.html'; // Redirect to the insurance list
                    } else {
                        const data = await response.json();
                        alert(data.message || 'Failed to delete insurance.');
                    }
                } catch (error) {
                    alert('Error: ' + error.message);
                }
            }
        });
    
        // Back to the insurance list
        backBtn.addEventListener('click', () => {
            window.location.href = 'index.html'; // Redirect to the insurance list
        });
    
        // Fetch and display insurance details on page load
        fetchInsuranceDetails();
    });
    

    // User login
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const email = document.getElementById('login-email').value;
        const password = document.getElementById('login-password').value;

        try {
            const response = await fetch('https://localhost:7279/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password }),
            });

            if (response.ok) {
                const data = await response.json();
                authToken = data.token;
                localStorage.setItem('authToken', authToken);
                alert('Login successful!');
                authSection.style.display = 'none';
                insuranceSection.style.display = 'block';
                fetchAllInsurances();
            } else {
                const data = await response.json();
                alert(data.message || 'Login failed.');
            }
        } catch (error) {
            alert('Error: ' + error.message);
        }
    });

    // Create insurance
    createInsuranceForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const type = document.getElementById('type').value;
        const method = document.getElementById('method').value;
        const status = document.getElementById('status').value;
        let price = document.getElementById('price').value;

        price = parseFloat(price);
        if (isNaN(price)) {
            alert('Please enter a valid price.');
            return;
        }

        try {
            const response = await fetch('https://localhost:7279/controller/create', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${authToken}`,
                },
                body: JSON.stringify({ type, method, status, price }),
            });

            if (response.ok) {
                alert('Insurance created successfully!');
                fetchAllInsurances();
            } else {
                const data = await response.json();
                alert(data.message || 'Failed to create insurance.');
            }
        } catch (error) {
            alert('Error: ' + error.message);
        }
    });

    // Back to login/register
    backToAuthBtn.addEventListener('click', () => {
        insuranceSection.style.display = 'none';
        authSection.style.display = 'block';
    });
});
