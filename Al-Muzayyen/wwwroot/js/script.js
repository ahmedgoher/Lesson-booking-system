document.addEventListener('DOMContentLoaded', function () {

    /* ==========================================
       1. الوضع الداكن / الفاتح (Dark هو الافتراضي)
       ========================================== */
    const html = document.documentElement;
    const themeToggle = document.getElementById('theme-toggle');
    const themeIcon = themeToggle ? themeToggle.querySelector('i') : null;

    function applyTheme(theme) {
        html.setAttribute('data-theme', theme);
        if (themeIcon) {
            themeIcon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
        }
        localStorage.setItem('site-theme', theme);
    }

    // الافتراضي Dark إلا لو المستخدم اختار قبل كده
    const savedTheme = localStorage.getItem('site-theme') || 'dark';
    applyTheme(savedTheme);

    if (themeToggle) {
        themeToggle.addEventListener('click', function () {
            const current = html.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
            applyTheme(current);
        });
    }

    /* ==========================================
       2. أنيميشن الظهور عند التمرير (Scroll Reveal)
       ========================================== */
    const revealEls = document.querySelectorAll('.animate-on-scroll');

    const revealObserver = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add('in-view');
                revealObserver.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.15,
        rootMargin: '0px 0px -60px 0px'
    });

    revealEls.forEach((el) => revealObserver.observe(el));

    /* ==========================================
       3. عداد الأرقام (Count Up) في سكشن الإحصائيات
       ========================================== */
    const counters = document.querySelectorAll('.counter');

    function animateCounter(el) {
        const target = parseInt(el.getAttribute('data-target'), 10) || 0;
        const suffix = el.getAttribute('data-suffix') || '';
        const duration = 1600;
        const startTime = performance.now();

        function step(now) {
            const progress = Math.min((now - startTime) / duration, 1);
            const eased = 1 - Math.pow(1 - progress, 3); // ease-out
            const value = Math.floor(eased * target);
            el.textContent = value.toLocaleString('ar-EG') + suffix;

            if (progress < 1) {
                requestAnimationFrame(step);
            } else {
                el.textContent = target.toLocaleString('ar-EG') + suffix;
            }
        }
        requestAnimationFrame(step);
    }

    if (counters.length) {
        const counterObserver = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    animateCounter(entry.target);
                    counterObserver.unobserve(entry.target);
                }
            });
        }, { threshold: 0.4 });

        counters.forEach((counter) => counterObserver.observe(counter));
    }

    /* ==========================================
       4. جزيئات عائمة في الهيرو
       ========================================== */
    const particlesContainer = document.getElementById('hero-particles');
    if (particlesContainer) {
        const particleCount = 18;
        for (let i = 0; i < particleCount; i++) {
            const span = document.createElement('span');
            const left = Math.random() * 100;
            const size = 4 + Math.random() * 6;
            const duration = 6 + Math.random() * 6;
            const delay = Math.random() * 8;

            span.style.left = left + '%';
            span.style.width = size + 'px';
            span.style.height = size + 'px';
            span.style.animationDuration = duration + 's';
            span.style.animationDelay = delay + 's';

            particlesContainer.appendChild(span);
        }
    }

});


document.addEventListener('DOMContentLoaded', function () {
    const dropdownTriggers = document.querySelectorAll('.dropdown-trigger');

    dropdownTriggers.forEach(trigger => {
        trigger.addEventListener('click', function (e) {
            e.stopPropagation();
            const currentDropdown = this.nextElementSibling;

            // إغلاق أي قائمة أخرى مفتوحة
            document.querySelectorAll('.dropdown-menu').forEach(menu => {
                if (menu !== currentDropdown) {
                    menu.classList.remove('show');
                }
            });

            // فتح أو إغلاق القائمة الحالية
            if (currentDropdown) {
                currentDropdown.classList.toggle('show');
            }
        });
    });

    // إغلاق القوائم المفتوحة عند الضغط في أي مكان خارجها
    document.addEventListener('click', function () {
        document.querySelectorAll('.dropdown-menu').forEach(menu => {
            menu.classList.remove('show');
        });
    });
});