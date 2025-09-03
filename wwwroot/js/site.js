// Enhanced JavaScript for BookStore
// Site-wide functionality

document.addEventListener('DOMContentLoaded', function () {
    initializeToasts();
    initializeCartFunctionality();
    initializeSearchFunctionality();
    initializeFormValidations();
    initializeLazyLoading();
    initializeAnimations();
});

// Toast Management
function initializeToasts() {
    // Auto-hide toasts after 5 seconds
    const toasts = document.querySelectorAll('.toast');
    toasts.forEach(toast => {
        setTimeout(() => {
            toast.classList.remove('show');
        }, 5000);
    });
}

// Cart Functionality
function initializeCartFunctionality() {
    // Add to cart buttons
    document.querySelectorAll('.add-to-cart-btn').forEach(button => {
        button.addEventListener('click', function () {
            const bookId = this.dataset.bookId;
            const quantity = this.closest('.card-body')?.querySelector('input[name="quantity"]')?.value || 1;

            addToCart(bookId, quantity, this);
        });
    });

    // Quantity update buttons
    document.querySelectorAll('.update-quantity-btn').forEach(button => {
        button.addEventListener('click', function () {
            const bookId = this.dataset.bookId;
            const action = this.dataset.action;
            const quantityInput = document.querySelector(`input[data-book-id="${bookId}"]`);

            let newQuantity = parseInt(quantityInput.value);
            const maxStock = parseInt(quantityInput.getAttribute('max')) || 999;

            if (action === 'increase' && newQuantity < maxStock) {
                newQuantity++;
            } else if (action === 'decrease' && newQuantity > 1) {
                newQuantity--;
            }

            quantityInput.value = newQuantity;
            updateCartQuantity(bookId, newQuantity);
        });
    });

    // Quantity input changes
    document.querySelectorAll('.quantity-input').forEach(input => {
        input.addEventListener('change', function () {
            const bookId = this.dataset.bookId;
            const quantity = parseInt(this.value);

            if (quantity > 0) {
                updateCartQuantity(bookId, quantity);
            }
        });
    });

    // Remove item buttons
    document.querySelectorAll('.remove-item-btn').forEach(button => {
        button.addEventListener('click', function () {
            const bookId = this.dataset.bookId;
            const bookTitle = this.closest('.cart-item')?.querySelector('h6')?.textContent || 'cet article';

            if (confirm(`Êtes-vous sûr de vouloir retirer "${bookTitle}" de votre panier ?`)) {
                removeFromCart(bookId);
            }
        });
    });
}

// Search and Filter Functionality
function initializeSearchFunctionality() {
    // Auto-submit search form with delay
    const searchInput = document.querySelector('input[name="SearchTerm"]');
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', function () {
            clearTimeout(searchTimeout);
            // Note: Auto-submit désactivé pour éviter trop de requêtes
            // Uncomment next lines to enable auto-search
            // searchTimeout = setTimeout(() => {
            //     this.form.submit();
            // }, 800);
        });
    }

    // Filter toggles
    document.querySelectorAll('.filter-toggle').forEach(toggle => {
        toggle.addEventListener('change', function () {
            this.form.submit();
        });
    });

    // Price range inputs
    const minPriceInput = document.querySelector('input[name="MinPrice"]');
    const maxPriceInput = document.querySelector('input[name="MaxPrice"]');

    if (minPriceInput && maxPriceInput) {
        [minPriceInput, maxPriceInput].forEach(input => {
            input.addEventListener('change', function () {
                const min = parseFloat(minPriceInput.value) || 0;
                const max = parseFloat(maxPriceInput.value) || 999;

                if (min > max) {
                    showToast('warning', 'Attention', 'Le prix minimum ne peut pas être supérieur au prix maximum.');
                }
            });
        });
    }
}

// Form Validations
function initializeFormValidations() {
    // Password confirmation validation
    const passwordField = document.querySelector('input[name="Password"]');
    const confirmPasswordField = document.querySelector('input[name="ConfirmPassword"]');

    if (passwordField && confirmPasswordField) {
        confirmPasswordField.addEventListener('input', function () {
            if (this.value !== passwordField.value) {
                this.setCustomValidity('Les mots de passe ne correspondent pas');
                this.classList.add('is-invalid');
            } else {
                this.setCustomValidity('');
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            }
        });
    }

    // Real-time validation feedback
    document.querySelectorAll('input[required], select[required]').forEach(field => {
        field.addEventListener('blur', function () {
            if (this.checkValidity()) {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else {
                this.classList.remove('is-valid');
                this.classList.add('is-invalid');
            }
        });
    });

    // Email format validation
    document.querySelectorAll('input[type="email"]').forEach(emailField => {
        emailField.addEventListener('input', function () {
            const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (this.value && !emailPattern.test(this.value)) {
                this.setCustomValidity('Veuillez entrer une adresse email valide');
                this.classList.add('is-invalid');
            } else {
                this.setCustomValidity('');
                this.classList.remove('is-invalid');
                if (this.value) this.classList.add('is-valid');
            }
        });
    });
}

// Cart API Functions
async function addToCart(bookId, quantity, buttonElement) {
    try {
        // Show loading state
        const originalText = buttonElement.innerHTML;
        buttonElement.disabled = true;
        buttonElement.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Ajout...';

        const response = await fetch('/Books/AddToCart', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: `bookId=${bookId}&quantity=${quantity}`
        });

        const data = await response.json();

        if (data.success) {
            // Update cart count in navbar
            updateNavbarCartCount(data.cartCount);

            // Show success feedback
            showToast('success', 'Succès', data.message);

            // Animate button
            buttonElement.innerHTML = '<i class="fas fa-check me-2"></i>Ajouté !';
            buttonElement.classList.remove('btn-primary');
            buttonElement.classList.add('btn-success');

            setTimeout(() => {
                buttonElement.innerHTML = originalText;
                buttonElement.classList.remove('btn-success');
                buttonElement.classList.add('btn-primary');
                buttonElement.disabled = false;
            }, 2000);
        } else {
            showToast('error', 'Erreur', data.message);
            buttonElement.innerHTML = originalText;
            buttonElement.disabled = false;
        }
    } catch (error) {
        console.error('Cart error:', error);
        showToast('error', 'Erreur', 'Une erreur de connexion est survenue.');
        buttonElement.innerHTML = originalText;
        buttonElement.disabled = false;
    }
}

async function updateCartQuantity(bookId, quantity) {
    try {
        const response = await fetch('/Cart/Update', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: `bookId=${bookId}&quantity=${quantity}`
        });

        const data = await response.json();

        if (data.success) {
            // Update totals in the UI
            updateCartTotals(data.newTotal, data.newItemsCount);
            updateNavbarCartCount(data.newItemsCount);
        } else {
            showToast('error', 'Erreur', data.message);
            // Reset quantity to previous value
            location.reload();
        }
    } catch (error) {
        console.error('Update quantity error:', error);
        showToast('error', 'Erreur', 'Erreur lors de la mise à jour.');
        location.reload();
    }
}

async function removeFromCart(bookId) {
    try {
        const response = await fetch('/Cart/Remove', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: `bookId=${bookId}`
        });

        const data = await response.json();

        if (data.success) {
            // Remove the item from UI with animation
            const cartItem = document.querySelector(`[data-book-id="${bookId}"]`).closest('.cart-item');
            if (cartItem) {
                cartItem.style.transition = 'all 0.3s ease';
                cartItem.style.opacity = '0';
                cartItem.style.transform = 'translateX(-100%)';

                setTimeout(() => {
                    cartItem.remove();
                    updateCartTotals(data.newTotal, data.newItemsCount);
                    updateNavbarCartCount(data.newItemsCount);

                    // Check if cart is empty
                    if (data.newItemsCount === 0) {
                        setTimeout(() => location.reload(), 500);
                    }
                }, 300);
            }

            showToast('success', 'Succès', data.message);
        } else {
            showToast('error', 'Erreur', 'Erreur lors de la suppression.');
        }
    } catch (error) {
        console.error('Remove item error:', error);
        showToast('error', 'Erreur', 'Erreur de connexion.');
    }
}

// UI Update Functions
function updateCartTotals(newTotal, newItemsCount) {
    const subtotalElement = document.getElementById('cart-subtotal');
    const totalElement = document.getElementById('cart-total');

    if (subtotalElement) subtotalElement.textContent = formatPrice(newTotal);
    if (totalElement) totalElement.textContent = formatPrice(newTotal);

    // Update items count
    const itemsCountElements = document.querySelectorAll('.items-count');
    itemsCountElements.forEach(el => {
        el.textContent = newItemsCount;
    });
}

function updateNavbarCartCount(count) {
    const cartBadge = document.querySelector('.navbar .badge');
    const cartLink = document.querySelector('.navbar .fa-shopping-cart').closest('a');

    if (count > 0) {
        if (cartBadge) {
            cartBadge.textContent = count;
            // Animate badge
            cartBadge.style.animation = 'bounce 0.5s ease';
            setTimeout(() => cartBadge.style.animation = '', 500);
        } else {
            // Create badge
            const badge = document.createElement('span');
            badge.className = 'position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger';
            badge.textContent = count;
            cartLink.appendChild(badge);
        }
    } else {
        if (cartBadge) {
            cartBadge.remove();
        }
    }
}

// Toast Utilities
function showToast(type, title, message) {
    const toastContainer = document.querySelector('.toast-container') || createToastContainer();
    const toast = createToast(type, title, message);
    toastContainer.appendChild(toast);

    // Show toast with animation
    setTimeout(() => {
        toast.classList.add('show');
    }, 100);

    // Auto-hide after 5 seconds
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => {
            if (toast.parentNode) {
                toast.remove();
            }
        }, 500);
    }, 5000);
}

function createToastContainer() {
    const container = document.createElement('div');
    container.className = 'toast-container position-fixed top-0 end-0 p-3';
    container.style.zIndex = '1055';
    document.body.appendChild(container);
    return container;
}

function createToast(type, title, message) {
    const colorClass = {
        'success': 'bg-success',
        'error': 'bg-danger',
        'warning': 'bg-warning',
        'info': 'bg-info'
    }[type] || 'bg-info';

    const icon = {
        'success': 'fa-check-circle',
        'error': 'fa-exclamation-triangle',
        'warning': 'fa-exclamation-triangle',
        'info': 'fa-info-circle'
    }[type] || 'fa-info-circle';

    const toast = document.createElement('div');
    toast.className = 'toast';
    toast.style.minWidth = '300px';
    toast.innerHTML = `
        <div class="toast-header ${colorClass} text-white">
            <i class="fas ${icon} me-2"></i>
            <strong class="me-auto">${title}</strong>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
        </div>
        <div class="toast-body">${message}</div>
    `;

    // Add click to dismiss functionality
    toast.querySelector('.btn-close').addEventListener('click', () => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 500);
    });

    return toast;
}

// Utility Functions
function getAntiForgeryToken() {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    return token || '';
}

// Image lazy loading for book covers
function initializeLazyLoading() {
    const images = document.querySelectorAll('img[data-src]');

    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const image = entry.target;
                    image.src = image.dataset.src;
                    image.classList.remove('lazy');
                    imageObserver.unobserve(image);
                }
            });
        });

        images.forEach(img => imageObserver.observe(img));
    } else {
        // Fallback for older browsers
        images.forEach(img => {
            if (img.dataset.src) {
                img.src = img.dataset.src;
            }
        });
    }
}

// Price formatting
function formatPrice(amount) {
    return new Intl.NumberFormat('fr-FR', {
        style: 'currency',
        currency: 'EUR'
    }).format(amount);
}

// Smooth animations for card interactions
function initializeAnimations() {
    // Animate cards on scroll
    const cards = document.querySelectorAll('.card');

    if ('IntersectionObserver' in window) {
        const cardObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-fade-in-up');
                }
            });
        }, {
            threshold: 0.1,
            rootMargin: '50px'
        });

        cards.forEach(card => cardObserver.observe(card));
    }
}

// Loading states for forms
document.querySelectorAll('form').forEach(form => {
    form.addEventListener('submit', function () {
        const submitButton = this.querySelector('button[type="submit"]');
        if (submitButton && !submitButton.disabled) {
            const originalText = submitButton.innerHTML;
            submitButton.disabled = true;
            submitButton.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Chargement...';

            // Re-enable after 10 seconds as fallback
            setTimeout(() => {
                if (submitButton.disabled) {
                    submitButton.innerHTML = originalText;
                    submitButton.disabled = false;
                }
            }, 10000);
        }
    });
});

// Keyboard shortcuts
document.addEventListener('keydown', function (e) {
    // Ctrl+K for search
    if (e.ctrlKey && e.key === 'k') {
        e.preventDefault();
        const searchInput = document.querySelector('input[name="SearchTerm"]');
        if (searchInput) {
            searchInput.focus();
            searchInput.select();
        }
    }

    // Escape to close modals
    if (e.key === 'Escape') {
        const openModals = document.querySelectorAll('.modal.show');
        openModals.forEach(modal => {
            const modalInstance = bootstrap.Modal.getInstance(modal);
            if (modalInstance) {
                modalInstance.hide();
            }
        });
    }
});

// Smooth scrolling for anchor links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// Enhanced book card interactions
document.querySelectorAll('.book-card').forEach(card => {
    // Hover effects
    card.addEventListener('mouseenter', function () {
        this.style.transform = 'translateY(-8px)';
    });

    card.addEventListener('mouseleave', function () {
        this.style.transform = 'translateY(0)';
    });
});

// Auto-expand textareas
document.querySelectorAll('textarea').forEach(textarea => {
    textarea.addEventListener('input', function () {
        this.style.height = 'auto';
        this.style.height = this.scrollHeight + 'px';
    });
});

// Price inputs formatting
document.querySelectorAll('input[type="number"][step="0.01"]').forEach(priceInput => {
    priceInput.addEventListener('blur', function () {
        if (this.value) {
            const value = parseFloat(this.value);
            this.value = value.toFixed(2);
        }
    });
});

// Confirmation dialogs for destructive actions
document.querySelectorAll('[data-confirm]').forEach(element => {
    element.addEventListener('click', function (e) {
        const message = this.dataset.confirm;
        if (!confirm(message)) {
            e.preventDefault();
            return false;
        }
    });
});

// Initialize tooltips (if Bootstrap tooltips are used)
function initializeTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(tooltipTriggerEl => {
        new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Call tooltip initialization
setTimeout(initializeTooltips, 100);

// Performance monitoring
function logPerformance() {
    if ('performance' in window) {
        window.addEventListener('load', function () {
            setTimeout(() => {
                const perfData = performance.timing;
                const loadTime = perfData.loadEventEnd - perfData.navigationStart;
                console.log(`Page loaded in ${loadTime}ms`);
            }, 0);
        });
    }
}

// Initialize performance monitoring
logPerformance();