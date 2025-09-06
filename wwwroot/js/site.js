// Enhanced JavaScript for BookStore - CORRECTED COMPLETE VERSION
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
    // Auto-hide existing toasts after 5 seconds
    const existingToasts = document.querySelectorAll('.toast');
    existingToasts.forEach(toast => {
        setTimeout(() => {
            toast.classList.remove('show');
        }, 5000);
    });
}

// Cart Functionality - CORRECTED
function initializeCartFunctionality() {
    // Add to cart buttons - CORRECTED to prevent double events
    document.querySelectorAll('.add-to-cart-btn').forEach(button => {
        // Remove any existing event listeners
        button.replaceWith(button.cloneNode(true));
    });

    // Re-add event listeners
    document.querySelectorAll('.add-to-cart-btn').forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();

            const bookId = this.dataset.bookId;

            // CORRECTED: Find quantity input - multiple possible locations
            let quantityInput = this.closest('.card-body')?.querySelector('input[name="quantity"]');
            if (!quantityInput) {
                quantityInput = this.closest('.input-group')?.querySelector('input[name="quantity"]');
            }
            if (!quantityInput) {
                quantityInput = document.querySelector(`#quantity-${bookId}`);
            }

            const quantity = quantityInput ? parseInt(quantityInput.value) || 1 : 1;

            addToCart(bookId, quantity, this);
        });
    });

    // Quantity update buttons
    document.querySelectorAll('.update-quantity-btn').forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();
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
        input.addEventListener('change', function (e) {
            const bookId = this.dataset.bookId;
            const quantity = parseInt(this.value);

            if (quantity > 0) {
                updateCartQuantity(bookId, quantity);
            }
        });
    });

    // Remove item buttons
    document.querySelectorAll('.remove-item-btn').forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();
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
            // Auto-search after 800ms of inactivity
            searchTimeout = setTimeout(() => {
                this.form.submit();
            }, 800);
        });
    }

    // Filter toggles
    document.querySelectorAll('.filter-toggle').forEach(toggle => {
        toggle.addEventListener('change', function () {
            this.form.submit();
        });
    });

    // Price range validation
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

// Cart API Functions - CORRECTED
async function addToCart(bookId, quantity, buttonElement) {
    // Prevent multiple clicks
    if (buttonElement.disabled) return;

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

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();

        if (data.success) {
            // CORRECTED: Update cart count in navbar
            updateNavbarCartCount(data.cartCount);

            // Show success feedback - FIXED: Only one toast
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

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();

        if (data.success) {
            // Update totals in the UI
            updateCartTotals(data.newTotal, data.newItemsCount);
            updateNavbarCartCount(data.newItemsCount);
        } else {
            showToast('error', 'Erreur', data.message);
            // Reset quantity to previous value
            setTimeout(() => location.reload(), 1000);
        }
    } catch (error) {
        console.error('Update quantity error:', error);
        showToast('error', 'Erreur', 'Erreur lors de la mise à jour.');
        setTimeout(() => location.reload(), 1000);
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

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

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

// CORRECTED: Update navbar cart count
function updateNavbarCartCount(count) {
    // Find the cart link - multiple possible selectors
    let cartLink = document.querySelector('.navbar .fa-shopping-cart');
    if (!cartLink) {
        cartLink = document.querySelector('a[href*="/Cart"]');
    }

    if (!cartLink) {
        console.error('Cart link not found in navbar');
        return;
    }

    // Get the parent link element
    const cartLinkElement = cartLink.closest('a');
    if (!cartLinkElement) {
        console.error('Cart link parent not found');
        return;
    }

    // Find existing badge
    let cartBadge = cartLinkElement.querySelector('.badge');

    if (count > 0) {
        if (cartBadge) {
            // Update existing badge
            cartBadge.textContent = count;
            // Add animation
            cartBadge.style.animation = 'none';
            setTimeout(() => {
                cartBadge.style.animation = 'bounce 0.5s ease';
            }, 10);
            setTimeout(() => {
                cartBadge.style.animation = '';
            }, 500);
        } else {
            // Create new badge
            cartBadge = document.createElement('span');
            cartBadge.className = 'position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger';
            cartBadge.textContent = count;
            cartBadge.style.fontSize = '0.75rem';

            // Make the cart link position relative for absolute positioning of badge
            cartLinkElement.style.position = 'relative';
            cartLinkElement.appendChild(cartBadge);

            console.log(`Created cart badge with count: ${count}`);
        }
    } else {
        if (cartBadge) {
            cartBadge.remove();
            console.log('Removed cart badge');
        }
    }
}

// Toast Utilities - IMPROVED to prevent duplicates
let activeToasts = new Set();

function showToast(type, title, message) {
    const toastKey = `${type}-${title}-${message}`;

    // Prevent duplicate toasts
    if (activeToasts.has(toastKey)) {
        return;
    }

    activeToasts.add(toastKey);

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
        activeToasts.delete(toastKey);
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
    const closeButton = toast.querySelector('.btn-close');
    if (closeButton) {
        closeButton.addEventListener('click', () => {
            toast.classList.remove('show');
            setTimeout(() => {
                if (toast.parentNode) {
                    toast.remove();
                }
            }, 500);
        });
    }

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

// Loading states for forms - IMPROVED
document.querySelectorAll('form').forEach(form => {
    form.addEventListener('submit', function (e) {
        const submitButton = this.querySelector('button[type="submit"]');
        if (submitButton && !submitButton.disabled && !submitButton.dataset.noLoading) {
            const originalText = submitButton.innerHTML;
            submitButton.disabled = true;
            submitButton.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Chargement...';

            // Store original text for restoration
            submitButton.dataset.originalText = originalText;

            // Re-enable after 10 seconds as fallback
            setTimeout(() => {
                if (submitButton.disabled && submitButton.dataset.originalText) {
                    submitButton.innerHTML = submitButton.dataset.originalText;
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

// FIXED: Handle form submissions properly for user registration
document.querySelectorAll('form').forEach(form => {
    // Add specific handling for registration form
    if (form.action.includes('Register')) {
        form.addEventListener('submit', function (e) {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.dataset.noLoading = 'true'; // Prevent double loading states
            }
        });
    }
});

// CSS Animation for badge bounce
const style = document.createElement('style');
style.textContent = `
    @keyframes bounce {
        0%, 20%, 60%, 100% {
            transform: translateY(0) translateX(-50%);
        }
        40% {
            transform: translateY(-10px) translateX(-50%);
        }
        80% {
            transform: translateY(-5px) translateX(-50%);
        }
    }
    
    .animate-fade-in-up {
        animation: fadeInUp 0.6s ease forwards;
    }
    
    @keyframes fadeInUp {
        from {
            opacity: 0;
            transform: translateY(30px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }
`;
document.head.appendChild(style);