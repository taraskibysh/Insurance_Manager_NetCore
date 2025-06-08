

document.addEventListener('DOMContentLoaded', () => {

    const API_BASE_URL = 'https://localhost:7279';
    let authToken = localStorage.getItem('authToken') || '';
    let userEmail = localStorage.getItem('userEmail') || '';

    const messageModal = document.getElementById('message-modal') || document.getElementById('message-modal-details');
    const messageModalTitle = document.getElementById('message-modal-title') || document.getElementById('message-modal-title-details');
    const messageModalText = document.getElementById('message-modal-text') || document.getElementById('message-modal-text-details');
    const messageModalCloseTop = document.getElementById('message-modal-close-top') || document.getElementById('message-modal-close-top-details');
    const messageModalCloseBottom = document.getElementById('message-modal-close-bottom') || document.getElementById('message-modal-close-bottom-details');

    function showMessage(title, text, isError = false) {
        if (!messageModal || !messageModalTitle || !messageModalText) {
            alert(`${title}: ${text}`);
            return;
        }
        messageModalTitle.textContent = title;
        messageModalText.textContent = text;
        messageModalTitle.classList.toggle('text-danger', isError); // Ensure .text-danger is defined in CSS
        messageModal.classList.remove('hidden');
    }

    function closeModal(modalElement) {
        if (modalElement) modalElement.classList.add('hidden');
    }

    if (messageModalCloseTop) messageModalCloseTop.addEventListener('click', () => closeModal(messageModal));
    if (messageModalCloseBottom) messageModalCloseBottom.addEventListener('click', () => closeModal(messageModal));

    const currentYearSpan = document.getElementById('current-year') || document.getElementById('current-year-details');
    if (currentYearSpan) currentYearSpan.textContent = new Date().getFullYear();

    const insuranceEnums = {
        TypeOfInsurance: [
            { value: 'CarInsurance', text: 'Автоцивілка (ОСЦПВ)' },
            { value: 'HealthInsurance', text: 'Медичне страхування' },
            { value: 'HouseInsurance', text: 'Страхування майна' },
            { value: 'AnotherInsurance', text: 'Інший тип' },
        ],
        MethodOfInsurance: [
            { value: 'FullInsurance', text: 'Повне покриття' },
            { value: 'HalfInsurance', text: 'Часткове покриття' },
            { value: 'QuarterInsurance', text: 'Базове покриття' },
        ],


        InsuranceStatus: [
            { value: 'Active', text: 'Активна' },
            { value: 'Canceled', text: 'Скасована' },
        ],
    };

    function populateSelect(selectElement, options, selectedValue = null) {
        if (!selectElement) return;
        const placeholder = selectElement.options[0];
        selectElement.innerHTML = '';
        if (placeholder && placeholder.disabled) selectElement.appendChild(placeholder);
        options.forEach(option => {
            const opt = document.createElement('option');
            opt.value = option.value;
            opt.textContent = option.text;
            if (selectedValue === option.value) opt.selected = true;
            selectElement.appendChild(opt);
        });
    }

    function updateGlobalUserUI() {
        const userGreeting = document.getElementById('user-greeting') || document.getElementById('user-greeting-details');
        const logoutButton = document.getElementById('logout-button') || document.getElementById('logout-button-details');

        if (authToken && userEmail) {
            if (userGreeting) {
                userGreeting.textContent = `Вітаємо, ${userEmail}!`;
                userGreeting.classList.remove('hidden');
            }
            if (logoutButton) logoutButton.classList.remove('hidden');
        } else {
            if (userGreeting) userGreeting.classList.add('hidden');
            if (logoutButton) logoutButton.classList.add('hidden');
        }
    }
    updateGlobalUserUI();

    function handleLogout() {
        authToken = '';
        userEmail = '';
        localStorage.removeItem('authToken');
        localStorage.removeItem('userEmail');
        updateGlobalUserUI();
        if (!window.location.pathname.endsWith('login.html')) {
            window.location.href = 'login.html';
        }
    }

    const logoutButton = document.getElementById('logout-button') || document.getElementById('logout-button-details');
    if (logoutButton) logoutButton.addEventListener('click', handleLogout);


    const currentPage = window.location.pathname;


    if (currentPage.endsWith('login.html')) {
        if (authToken) { 
            window.location.href = 'index.html';
            return; 
        }

        const loginFormEl = document.getElementById('login-form');
        if (loginFormEl) {
            loginFormEl.addEventListener('submit', async (e) => {
                e.preventDefault();
                const emailInput = document.getElementById('login-email').value;
                const password = document.getElementById('login-password').value;
                try {
                    const response = await fetch(`${API_BASE_URL}/auth/login`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ email: emailInput, password }),
                    });
                    if (response.ok) {
                        const data = await response.json();
                        authToken = data.token;
                        userEmail = emailInput;
                        localStorage.setItem('authToken', authToken);
                        localStorage.setItem('userEmail', userEmail);
                        window.location.href = 'index.html';
                    } else {
                        const data = await response.json().catch(() => ({ message: 'Login failed.' }));
                        showMessage('Помилка входу', data.message || 'Неправильний email або пароль.', true);
                    }
                } catch (error) {
                    showMessage('Помилка мережі', 'Не вдалося з\'єднатися з сервером.', true);
                }
            });
        }
    }
    
    else if (currentPage.endsWith('register.html')) {
        if (authToken) { 
            window.location.href = 'index.html';
            return;
        }
        const registerFormEl = document.getElementById('register-form');
        if (registerFormEl) {
            registerFormEl.addEventListener('submit', async (e) => {
                e.preventDefault();
                const firstName = document.getElementById('firstName').value;
                const lastName = document.getElementById('lastName').value;
                const email = document.getElementById('email').value;
                const password = document.getElementById('password').value;
                const confirmPassword = document.getElementById('confirm-password').value;

                if (password !== confirmPassword) {
                    showMessage('Помилка реєстрації', 'Паролі не співпадають.', true);
                    return;
                }
                if (password.length < 6) {
                    showMessage('Помилка реєстрації', 'Пароль повинен містити щонайменше 6 символів.', true);
                    return;
                }

                try {
                    const response = await fetch(`${API_BASE_URL}/auth/register`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ firstName, lastName, email, password }),
                    });
                    if (response.ok) {
                        showMessage('Успіх', 'Реєстрація успішна! Тепер ви можете увійти.');
                        setTimeout(() => { window.location.href = 'login.html'; }, 2000);
                    } else {
                        const data = await response.json().catch(() => ({ message: 'Registration failed.' }));
                        showMessage('Помилка реєстрації', data.message || 'Не вдалося зареєструватися.', true);
                    }
                } catch (error) {
                    showMessage('Помилка мережі', 'Не вдалося з\'єднатися з сервером.', true);
                }
            });
        }
    }


    else if (currentPage.endsWith('index.html') || currentPage === '/' || currentPage.endsWith('/index')) {
        if (!authToken) {
            const authRedirectMessage = document.getElementById('auth-redirect-message');
            if(authRedirectMessage) authRedirectMessage.classList.remove('hidden');
            console.log("User not authenticated. Showing redirect message or redirecting.");
            const insuranceSectionEl = document.getElementById('insurance-section');
            if (insuranceSectionEl) insuranceSectionEl.classList.add('hidden');
            
            return;
        }

        const insuranceSectionEl = document.getElementById('insurance-section');
        const authRedirectMessage = document.getElementById('auth-redirect-message');

        if (insuranceSectionEl) insuranceSectionEl.classList.remove('hidden');
        if (authRedirectMessage) authRedirectMessage.classList.add('hidden');


        const createInsuranceFormEl = document.getElementById('create-insurance-form');
        const insuranceListEl = document.getElementById('insurance-list');
        const typeSelect = document.getElementById('type');
        const methodSelect = document.getElementById('method');
        const statusSelect = document.getElementById('status');

        populateSelect(typeSelect, insuranceEnums.TypeOfInsurance);
        populateSelect(methodSelect, insuranceEnums.MethodOfInsurance);
        populateSelect(statusSelect, insuranceEnums.InsuranceStatus);

        async function fetchAllInsurances() {
            if (!authToken) return;
            try {
                const response = await fetch(`${API_BASE_URL}/controller/all`, {
                    method: 'GET',
                    headers: { 'Authorization': `Bearer ${authToken}` },
                });
                if (response.ok) {
                    const insurances = await response.json();
                    renderInsuranceList(insurances);
                } else {
                    if (response.status === 401) handleLogout(); // Token expired or invalid
                    else showMessage('Помилка', `Помилка завантаження страховок. Статус: ${response.status}`, true);
                }
            } catch (error) {
                showMessage('Помилка мережі', 'Не вдалося завантажити страховки.', true);
            }
        }

        function renderInsuranceList(insurances) {
            if (!insuranceListEl) return;
            insuranceListEl.innerHTML = '';
            if (!insurances || insurances.length === 0) {
                insuranceListEl.innerHTML = '<p class="empty-list-message">Немає доступних страховок.</p>';
                return;
            }
            insurances.forEach((insurance) => {
                const item = document.createElement('div');
                item.className = 'insurance-item';
                item.innerHTML = `
                    <h3>${insuranceEnums.TypeOfInsurance.find(t => t.value === insurance.typeOfInsurance)?.text || insurance.typeOfInsurance}</h3>
                    <p><strong>Статус:</strong> ${insuranceEnums.InsuranceStatus.find(s => s.value === insurance.status)?.text || insurance.status}</p>
                    <p><strong>Ціна:</strong> ${insurance.price.toFixed(2)} грн</p>
                    <span class="details-link">Детальніше...</span>`;
                item.addEventListener('click', () => window.location.href = `insurance_details.html?id=${insurance.id}`);
                insuranceListEl.appendChild(item);
            });
        }

        if (createInsuranceFormEl) {
            createInsuranceFormEl.addEventListener('submit', async (e) => {
                e.preventDefault();
                const type = document.getElementById('type').value;
                const method = document.getElementById('method').value;
                const status = document.getElementById('status').value;
                let price = parseFloat(document.getElementById('price').value);

                if (isNaN(price) || price <= 0) {
                    showMessage('Помилка валідації', 'Введіть коректну позитивну ціну.', true);
                    return;
                }
                try {
                    const response = await fetch(`${API_BASE_URL}/controller/create`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${authToken}` },
                        body: JSON.stringify({ typeOfInsurance: type, methodOfInsurance: method, status, price }),
                    });
                    if (response.ok) {
                        showMessage('Успіх', 'Страховку створено!');
                        fetchAllInsurances();
                        createInsuranceFormEl.reset();
                    } else {
                        if (response.status === 401) handleLogout();
                        else {
                            const data = await response.json().catch(() => ({ message: 'Failed to create.' }));
                            showMessage('Помилка створення', data.message || 'Не вдалося створити страховку.', true);
                        }
                    }
                } catch (error) {
                    showMessage('Помилка мережі', 'Не вдалося створити страховку.', true);
                }
            });
        }
        fetchAllInsurances(); 
    }
    
    else if (currentPage.includes('insurance_details.html')) {
        if (!authToken) {
            window.location.href = 'login.html';
            return;
        }

        const insuranceId = new URLSearchParams(window.location.search).get('id');
        if (!insuranceId) {
            showMessage('Помилка', 'ID страховки не знайдено.', true);
            setTimeout(() => window.location.href = 'index.html', 2000);
            return;
        }

        const detailsIdEl = document.getElementById('details-id');
        const detailsTypeEl = document.getElementById('details-type');
        const detailsMethodEl = document.getElementById('details-method');
        const detailsStatusEl = document.getElementById('details-status');
        const detailsPriceEl = document.getElementById('details-price');
        const detailsPayEl = document.getElementById('details-pay');

        const editBtn = document.getElementById('edit-btn');
        const deleteBtn = document.getElementById('delete-btn');
        const backBtn = document.getElementById('back-btn');

        const editInsuranceModal = document.getElementById('edit-insurance-modal');
        const editModalCloseTop = document.getElementById('edit-modal-close-top');
        const editModalCancelBottom = document.getElementById('edit-modal-cancel-bottom');
        const editForm = document.getElementById('edit-form');
        const editInsuranceIdInput = document.getElementById('edit-insurance-id');
        const editTypeSelect = document.getElementById('edit-type');
        const editMethodSelect = document.getElementById('edit-method');
        const editStatusSelect = document.getElementById('edit-status');
        const editPriceInput = document.getElementById('edit-price');
        const editPayInput = document.getElementById('edit-pay');

        populateSelect(editTypeSelect, insuranceEnums.TypeOfInsurance);
        populateSelect(editMethodSelect, insuranceEnums.MethodOfInsurance);
        populateSelect(editStatusSelect, insuranceEnums.InsuranceStatus);

        const deleteConfirmModal = document.getElementById('delete-confirm-modal');
        const deleteConfirmModalCloseTop = document.getElementById('delete-confirm-modal-close-top');
        const deleteConfirmModalConfirmBtn = document.getElementById('delete-confirm-modal-confirm-btn');
        const deleteConfirmModalCancelBtn = document.getElementById('delete-confirm-modal-cancel-btn');

        async function fetchInsuranceDetails() {
            try {
                const response = await fetch(`${API_BASE_URL}/controller/Id?id=${insuranceId}`, {
                    method: 'GET',
                    headers: { 'Authorization': `Bearer ${authToken}` },
                });
                if (!response.ok) {
                    if (response.status === 401) handleLogout();
                    else throw new Error(`Статус: ${response.status}`);
                }
                const insurance = await response.json();
                renderInsuranceDetails(insurance);
            } catch (error) {
                showMessage('Помилка', `Не вдалося завантажити деталі: ${error.message}`, true);
            }
        }

        function renderInsuranceDetails(insurance) {
            if (detailsIdEl) detailsIdEl.textContent = insurance.id;
            if (detailsTypeEl) detailsTypeEl.textContent = insuranceEnums.TypeOfInsurance.find(t => t.value === insurance.typeOfInsurance)?.text || insurance.typeOfInsurance;
            if (detailsMethodEl) detailsMethodEl.textContent = insuranceEnums.MethodOfInsurance.find(m => m.value === insurance.methodOfInsurance)?.text || insurance.methodOfInsurance;
            if (detailsStatusEl) detailsStatusEl.textContent = insuranceEnums.InsuranceStatus.find(s => s.value === insurance.status)?.text || insurance.status;
            if (detailsPriceEl) detailsPriceEl.textContent = insurance.price.toFixed(2);
            if (detailsPayEl) detailsPayEl.textContent = insurance.pay ? insurance.pay.toFixed(2) : '0.00';

            if (editInsuranceIdInput) editInsuranceIdInput.value = insurance.id;
            populateSelect(editTypeSelect, insuranceEnums.TypeOfInsurance, insurance.typeOfInsurance);
            populateSelect(editMethodSelect, insuranceEnums.MethodOfInsurance, insurance.methodOfInsurance);
            populateSelect(editStatusSelect, insuranceEnums.InsuranceStatus, insurance.status);
            if (editPriceInput) editPriceInput.value = insurance.price;
            if (editPayInput) editPayInput.value = insurance.pay || 0;
        }

        if (editBtn) {
            editBtn.addEventListener('click', () => {
                fetchInsuranceDetails().then(() => {
                    if (editInsuranceModal) editInsuranceModal.classList.remove('hidden');
                });
            });
        }
        if (editModalCloseTop) editModalCloseTop.addEventListener('click', () => closeModal(editInsuranceModal));
        if (editModalCancelBottom) editModalCancelBottom.addEventListener('click', () => closeModal(editInsuranceModal));

        if (editForm) {
            editForm.addEventListener('submit', async (e) => {
                e.preventDefault();
                const id = editInsuranceIdInput.value;
                const type = editTypeSelect.value;
                const method = editMethodSelect.value;
                const status = editStatusSelect.value;
                let price = parseFloat(editPriceInput.value);
                let pay = parseFloat(editPayInput.value);

                if (isNaN(price) || price <= 0) {
                    showMessage('Помилка', 'Введіть коректну позитивну ціну.', true); return;
                }
                if (isNaN(pay) || pay < 0) {
                    showMessage('Помилка', 'Введіть коректну суму оплати.', true); return;
                }
                try {
                    const response = await fetch(`${API_BASE_URL}/controller/Id?id=${id}`, {
                        method: 'PUT',
                        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${authToken}` },
                        body: JSON.stringify({ typeOfInsurance: type, methodOfInsurance: method, status, price, pay })
                    });
                    if (response.ok) {
                        showMessage('Успіх', 'Страховку оновлено!');
                        closeModal(editInsuranceModal);
                        fetchInsuranceDetails();
                    } else {
                        if (response.status === 401) handleLogout();
                        else {
                            const data = await response.json().catch(() => ({ message: 'Failed to update.' }));
                            showMessage('Помилка', data.message || 'Не вдалося оновити.', true);
                        }
                    }
                } catch (error) {
                    showMessage('Помилка мережі', 'Не вдалося оновити страховку.', true);
                }
            });
        }

        if (deleteBtn) {
            deleteBtn.addEventListener('click', () => {
                if (deleteConfirmModal) deleteConfirmModal.classList.remove('hidden');
            });
        }
        if (deleteConfirmModalCloseTop) deleteConfirmModalCloseTop.addEventListener('click', () => closeModal(deleteConfirmModal));
        if (deleteConfirmModalCancelBtn) deleteConfirmModalCancelBtn.addEventListener('click', () => closeModal(deleteConfirmModal));

        if (deleteConfirmModalConfirmBtn) {
            deleteConfirmModalConfirmBtn.addEventListener('click', async () => {
                try {
                    const response = await fetch(`${API_BASE_URL}/controller/Id?id=${insuranceId}`, {
                        method: 'DELETE',
                        headers: { 'Authorization': `Bearer ${authToken}` },
                    });
                    if (response.ok) {
                        showMessage('Успіх', 'Страховку видалено!');
                        closeModal(deleteConfirmModal);
                        setTimeout(() => window.location.href = 'index.html', 1500);
                    } else {
                        if (response.status === 401) handleLogout();
                        else {
                            const data = await response.json().catch(() => ({ message: 'Failed to delete.' }));
                            showMessage('Помилка', data.message || 'Не вдалося видалити.', true);
                        }
                        closeModal(deleteConfirmModal);
                    }
                } catch (error) {
                    showMessage('Помилка мережі', 'Не вдалося видалити страховку.', true);
                    closeModal(deleteConfirmModal);
                }
            });
        }

        if (backBtn) backBtn.addEventListener('click', () => window.location.href = 'index.html');

        fetchInsuranceDetails();
    }
});
