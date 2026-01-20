document.addEventListener('DOMContentLoaded', function () {
    const currentPath = window.location.pathname;
    const sidebarLinks = document.querySelectorAll('.sidebar-menu-link');

    sidebarLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.style.backgroundColor = 'rgba(255, 255, 255, 0.1)';
            link.style.color = '#FFFFFF';
            link.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.2)';
        }
    });

    const neoCards = document.querySelectorAll('.neo-card');

    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver(function (entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '0';
                entry.target.style.transform = 'translateY(20px)';

                setTimeout(() => {
                    entry.target.style.transition = 'all 0.5s ease';
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }, 100);

                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    neoCards.forEach(card => {
        observer.observe(card);
    });

    const neoButtons = document.querySelectorAll('.neo-btn');

    neoButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            const ripple = document.createElement('span');
            ripple.style.position = 'absolute';
            ripple.style.width = '20px';
            ripple.style.height = '20px';
            ripple.style.background = 'rgba(0, 0, 0, 0.3)';
            ripple.style.borderRadius = '50%';
            ripple.style.transform = 'scale(0)';
            ripple.style.animation = 'ripple 0.6s ease-out';

            const rect = button.getBoundingClientRect();
            ripple.style.left = (e.clientX - rect.left) + 'px';
            ripple.style.top = (e.clientY - rect.top) + 'px';

            button.style.position = 'relative';
            button.style.overflow = 'hidden';
            button.appendChild(ripple);

            setTimeout(() => ripple.remove(), 600);
        });
    });
});

function toggleMobileNav() {
    const mobileNav = document.getElementById('mobileNav');
    const toggleIcon = document.querySelector('#sidebarToggle i');

    mobileNav.classList.toggle('show');

    if (mobileNav.classList.contains('show')) {
        toggleIcon.classList.replace('bi-list', 'bi-x-lg');
        document.body.style.overflow = 'hidden'; // Prevent scrolling when menu is open
    } else {
        toggleIcon.classList.replace('bi-x-lg', 'bi-list');
        document.body.style.overflow = 'auto';
    }
}

// Close mobile nav when clicking a link
document.addEventListener('DOMContentLoaded', function () {
    const mobileLinks = document.querySelectorAll('.mobile-nav-link');
    mobileLinks.forEach(link => {
        link.addEventListener('click', () => {
            const mobileNav = document.getElementById('mobileNav');
            if (mobileNav.classList.contains('show')) {
                toggleMobileNav();
            }
        });
    });
});

const style = document.createElement('style');
style.textContent = `
    @keyframes ripple {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);
