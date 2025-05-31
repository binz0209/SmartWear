/**
* Template Name: FashionStore
* Template URL: https://bootstrapmade.com/fashion-store-bootstrap-template/
* Updated: Apr 26 2025 with Bootstrap v5.3.5
* Author: BootstrapMade.com
* License: https://bootstrapmade.com/license/
*/

(function() {
  "use strict";

  /**
   * Apply .scrolled class to the body as the page is scrolled down
   */
  function toggleScrolled() {
    const selectBody = document.querySelector('body');
    const selectHeader = document.querySelector('#header');
    if (!selectHeader.classList.contains('scroll-up-sticky') && !selectHeader.classList.contains('sticky-top') && !selectHeader.classList.contains('fixed-top')) return;
    window.scrollY > 100 ? selectBody.classList.add('scrolled') : selectBody.classList.remove('scrolled');
  }

  document.addEventListener('scroll', toggleScrolled);
  window.addEventListener('load', toggleScrolled);

  /**
   * Init swiper sliders
   */
  function initSwiper() {
    document.querySelectorAll(".init-swiper").forEach(function(swiperElement) {
      let config = JSON.parse(
        swiperElement.querySelector(".swiper-config").innerHTML.trim()
      );

      if (swiperElement.classList.contains("swiper-tab")) {
        initSwiperWithCustomPagination(swiperElement, config);
      } else {
        new Swiper(swiperElement, config);
      }
    });
  }

  window.addEventListener("load", initSwiper);

  /**
   * Mobile nav toggle
   */
  const mobileNavToggleBtn = document.querySelector('.mobile-nav-toggle');

  function mobileNavToogle() {
    document.querySelector('body').classList.toggle('mobile-nav-active');
    mobileNavToggleBtn.classList.toggle('bi-list');
    mobileNavToggleBtn.classList.toggle('bi-x');
  }
  if (mobileNavToggleBtn) {
    mobileNavToggleBtn.addEventListener('click', mobileNavToogle);
  }

  /**
   * Hide mobile nav on same-page/hash links
   */
  document.querySelectorAll('#navmenu a').forEach(navmenu => {
    navmenu.addEventListener('click', () => {
      if (document.querySelector('.mobile-nav-active')) {
        mobileNavToogle();
      }
    });

  });

  /**
   * Toggle mobile nav dropdowns
   */
  document.querySelectorAll('.navmenu .toggle-dropdown').forEach(navmenu => {
    navmenu.addEventListener('click', function(e) {
      e.preventDefault();
      this.parentNode.classList.toggle('active');
      this.parentNode.nextElementSibling.classList.toggle('dropdown-active');
      e.stopImmediatePropagation();
    });
  });

  /**
   * Preloader
   */
  const preloader = document.querySelector('#preloader');
  if (preloader) {
    window.addEventListener('load', () => {
      preloader.remove();
    });
  }

  /**
   * Scroll top button
   */
  let scrollTop = document.querySelector('.scroll-top');

  function toggleScrollTop() {
    if (scrollTop) {
      window.scrollY > 100 ? scrollTop.classList.add('active') : scrollTop.classList.remove('active');
    }
  }
  scrollTop.addEventListener('click', (e) => {
    e.preventDefault();
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  });

  window.addEventListener('load', toggleScrollTop);
  document.addEventListener('scroll', toggleScrollTop);

  /**
   * Animation on scroll function and init
   */
  function aosInit() {
    AOS.init({
      duration: 600,
      easing: 'ease-in-out',
      once: true,
      mirror: false
    });
  }
  window.addEventListener('load', aosInit);

  /**
   * Countdown timer
   */
  function updateCountDown(countDownItem) {
    const timeleft = new Date(countDownItem.getAttribute('data-count')).getTime() - new Date().getTime();

    const days = Math.floor(timeleft / (1000 * 60 * 60 * 24));
    const hours = Math.floor((timeleft % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((timeleft % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((timeleft % (1000 * 60)) / 1000);

    countDownItem.querySelector('.count-days').innerHTML = days;
    countDownItem.querySelector('.count-hours').innerHTML = hours;
    countDownItem.querySelector('.count-minutes').innerHTML = minutes;
    countDownItem.querySelector('.count-seconds').innerHTML = seconds;

  }

  document.querySelectorAll('.countdown').forEach(function(countDownItem) {
    updateCountDown(countDownItem);
    setInterval(function() {
      updateCountDown(countDownItem);
    }, 1000);
  });

  /**
   * Init isotope layout and filters
   */
  document.querySelectorAll('.isotope-layout').forEach(function(isotopeItem) {
    let layout = isotopeItem.getAttribute('data-layout') ?? 'masonry';
    let filter = isotopeItem.getAttribute('data-default-filter') ?? '*';
    let sort = isotopeItem.getAttribute('data-sort') ?? 'original-order';

    let initIsotope;
    imagesLoaded(isotopeItem.querySelector('.isotope-container'), function() {
      initIsotope = new Isotope(isotopeItem.querySelector('.isotope-container'), {
        itemSelector: '.isotope-item',
        layoutMode: layout,
        filter: filter,
        sortBy: sort
      });
    });

    isotopeItem.querySelectorAll('.isotope-filters li').forEach(function(filters) {
      filters.addEventListener('click', function() {
        isotopeItem.querySelector('.isotope-filters .filter-active').classList.remove('filter-active');
        this.classList.add('filter-active');
        initIsotope.arrange({
          filter: this.getAttribute('data-filter')
        });
        if (typeof aosInit === 'function') {
          aosInit();
        }
      }, false);
    });

  });

  /**
   * Ecommerce Cart Functionality
   * Handles quantity changes and item removal
   */

  function ecommerceCartTools() {
    // Get all quantity buttons and inputs directly
    const decreaseButtons = document.querySelectorAll('.quantity-btn.decrease');
    const increaseButtons = document.querySelectorAll('.quantity-btn.increase');
    const quantityInputs = document.querySelectorAll('.quantity-input');
    const removeButtons = document.querySelectorAll('.remove-item');

    // Decrease quantity buttons
    decreaseButtons.forEach(btn => {
      btn.addEventListener('click', function() {
        const quantityInput = btn.closest('.quantity-selector').querySelector('.quantity-input');
        let currentValue = parseInt(quantityInput.value);
        if (currentValue > 1) {
          quantityInput.value = currentValue - 1;
        }
      });
    });

    // Increase quantity buttons
    increaseButtons.forEach(btn => {
      btn.addEventListener('click', function() {
        const quantityInput = btn.closest('.quantity-selector').querySelector('.quantity-input');
        let currentValue = parseInt(quantityInput.value);
        if (currentValue < parseInt(quantityInput.getAttribute('max'))) {
          quantityInput.value = currentValue + 1;
        }
      });
    });

    // Manual quantity inputs
    quantityInputs.forEach(input => {
      input.addEventListener('change', function() {
        let currentValue = parseInt(input.value);
        const min = parseInt(input.getAttribute('min'));
        const max = parseInt(input.getAttribute('max'));

        // Validate input
        if (isNaN(currentValue) || currentValue < min) {
          input.value = min;
        } else if (currentValue > max) {
          input.value = max;
        }
      });
    });

    // Remove item buttons
    removeButtons.forEach(btn => {
      btn.addEventListener('click', function() {
        btn.closest('.cart-item').remove();
      });
    });
  }

  ecommerceCartTools();

  /**
   * Product Image Zoom and Thumbnail Functionality
   */

  function productDetailFeatures() {
    // Initialize Drift for image zoom
    function initDriftZoom() {
      // Check if Drift is available
      if (typeof Drift === 'undefined') {
        console.error('Drift library is not loaded');
        return;
      }

      const driftOptions = {
        paneContainer: document.querySelector('.image-zoom-container'),
        inlinePane: window.innerWidth < 768 ? true : false,
        inlineOffsetY: -85,
        containInline: true,
        hoverBoundingBox: false,
        zoomFactor: 3,
        handleTouch: false
      };

      // Initialize Drift on the main product image
      const mainImage = document.getElementById('main-product-image');
      if (mainImage) {
        new Drift(mainImage, driftOptions);
      }
    }

    // Thumbnail click functionality
    function initThumbnailClick() {
      const thumbnails = document.querySelectorAll('.thumbnail-item');
      const mainImage = document.getElementById('main-product-image');

      if (!thumbnails.length || !mainImage) return;

      thumbnails.forEach(thumbnail => {
        thumbnail.addEventListener('click', function() {
          // Get image path from data attribute
          const imageSrc = this.getAttribute('data-image');

          // Update main image src and zoom attribute
          mainImage.src = imageSrc;
          mainImage.setAttribute('data-zoom', imageSrc);

          // Update active state
          thumbnails.forEach(item => item.classList.remove('active'));
          this.classList.add('active');

          // Reinitialize Drift for the new image
          initDriftZoom();
        });
      });
    }

    // Image navigation functionality (prev/next buttons)
    function initImageNavigation() {
      const prevButton = document.querySelector('.image-nav-btn.prev-image');
      const nextButton = document.querySelector('.image-nav-btn.next-image');

      if (!prevButton || !nextButton) return;

      const thumbnails = Array.from(document.querySelectorAll('.thumbnail-item'));
      if (!thumbnails.length) return;

      // Function to navigate to previous or next image
      function navigateImage(direction) {
        // Find the currently active thumbnail
        const activeIndex = thumbnails.findIndex(thumb => thumb.classList.contains('active'));
        if (activeIndex === -1) return;

        let newIndex;
        if (direction === 'prev') {
          // Go to previous image or loop to the last one
          newIndex = activeIndex === 0 ? thumbnails.length - 1 : activeIndex - 1;
        } else {
          // Go to next image or loop to the first one
          newIndex = activeIndex === thumbnails.length - 1 ? 0 : activeIndex + 1;
        }

        // Simulate click on the new thumbnail
        thumbnails[newIndex].click();
      }

      // Add event listeners to navigation buttons
      prevButton.addEventListener('click', () => navigateImage('prev'));
      nextButton.addEventListener('click', () => navigateImage('next'));
    }

    // Initialize all features
    initDriftZoom();
    initThumbnailClick();
    initImageNavigation();
  }

  productDetailFeatures();

  /**
   * Price range slider implementation for price filtering.
   */
  function priceRangeWidget() {
    // Get all price range widgets on the page
    const priceRangeWidgets = document.querySelectorAll('.price-range-container');

    priceRangeWidgets.forEach(widget => {
      const minRange = widget.querySelector('.min-range');
      const maxRange = widget.querySelector('.max-range');
      const sliderProgress = widget.querySelector('.slider-progress');
      const minPriceDisplay = widget.querySelector('.current-range .min-price');
      const maxPriceDisplay = widget.querySelector('.current-range .max-price');
      const minPriceInput = widget.querySelector('.min-price-input');
      const maxPriceInput = widget.querySelector('.max-price-input');
      const applyButton = widget.querySelector('.filter-actions .btn-primary');

      if (!minRange || !maxRange || !sliderProgress || !minPriceDisplay || !maxPriceDisplay || !minPriceInput || !maxPriceInput) return;

      // Slider configuration
      const sliderMin = parseInt(minRange.min);
      const sliderMax = parseInt(minRange.max);
      const step = parseInt(minRange.step) || 1;

      // Initialize with default values
      let minValue = parseInt(minRange.value);
      let maxValue = parseInt(maxRange.value);

      // Set initial values
      updateSliderProgress();
      updateDisplays();

      // Min range input event
      minRange.addEventListener('input', function() {
        minValue = parseInt(this.value);

        // Ensure min doesn't exceed max
        if (minValue > maxValue) {
          minValue = maxValue;
          this.value = minValue;
        }

        // Update min price input and display
        minPriceInput.value = minValue;
        updateDisplays();
        updateSliderProgress();
      });

      // Max range input event
      maxRange.addEventListener('input', function() {
        maxValue = parseInt(this.value);

        // Ensure max isn't less than min
        if (maxValue < minValue) {
          maxValue = minValue;
          this.value = maxValue;
        }

        // Update max price input and display
        maxPriceInput.value = maxValue;
        updateDisplays();
        updateSliderProgress();
      });

      // Min price input change
      minPriceInput.addEventListener('change', function() {
        let value = parseInt(this.value) || sliderMin;

        // Ensure value is within range
        value = Math.max(sliderMin, Math.min(sliderMax, value));

        // Ensure min doesn't exceed max
        if (value > maxValue) {
          value = maxValue;
        }

        // Update min value and range input
        minValue = value;
        this.value = value;
        minRange.value = value;
        updateDisplays();
        updateSliderProgress();
      });

      // Max price input change
      maxPriceInput.addEventListener('change', function() {
        let value = parseInt(this.value) || sliderMax;

        // Ensure value is within range
        value = Math.max(sliderMin, Math.min(sliderMax, value));

        // Ensure max isn't less than min
        if (value < minValue) {
          value = minValue;
        }

        // Update max value and range input
        maxValue = value;
        this.value = value;
        maxRange.value = value;
        updateDisplays();
        updateSliderProgress();
      });

      // Apply button click
      if (applyButton) {
        applyButton.addEventListener('click', function() {
          // This would typically trigger a form submission or AJAX request
          console.log(`Applying price filter: $${minValue} - $${maxValue}`);

          // Here you would typically add code to filter products or redirect to a filtered URL
        });
      }

      // Helper function to update the slider progress bar
      function updateSliderProgress() {
        const range = sliderMax - sliderMin;
        const minPercent = ((minValue - sliderMin) / range) * 100;
        const maxPercent = ((maxValue - sliderMin) / range) * 100;

        sliderProgress.style.left = `${minPercent}%`;
        sliderProgress.style.width = `${maxPercent - minPercent}%`;
      }

      // Helper function to update price displays
      function updateDisplays() {
        minPriceDisplay.textContent = `$${minValue}`;
        maxPriceDisplay.textContent = `$${maxValue}`;
      }
    });
  }
  priceRangeWidget();

  /**
   * Ecommerce Checkout Section
   * This script handles the functionality of both multi-step and one-page checkout processes
   */

  function initCheckout() {
    // Detect checkout type
    const isMultiStepCheckout = document.querySelector('.checkout-steps') !== null;
    const isOnePageCheckout = document.querySelector('.checkout-section') !== null;

    // Initialize common functionality
    initInputMasks();
    initPromoCode();

    // Initialize checkout type specific functionality
    if (isMultiStepCheckout) {
      initMultiStepCheckout();
    }

    if (isOnePageCheckout) {
      initOnePageCheckout();
    }

    // Initialize tooltips (works for both checkout types)
    initTooltips();
  }

  initCheckout();

  // Function to initialize multi-step checkout
  function initMultiStepCheckout() {
    // Get all checkout elements
    const checkoutSteps = document.querySelectorAll('.checkout-steps .step');
    const checkoutForms = document.querySelectorAll('.checkout-form');
    const nextButtons = document.querySelectorAll('.next-step');
    const prevButtons = document.querySelectorAll('.prev-step');
    const editButtons = document.querySelectorAll('.btn-edit');
    const paymentMethods = document.querySelectorAll('.payment-method-header');
    const summaryToggle = document.querySelector('.btn-toggle-summary');
    const orderSummaryContent = document.querySelector('.order-summary-content');

    // Step Navigation
    nextButtons.forEach(button => {
      button.addEventListener('click', function() {
        const nextStep = parseInt(this.getAttribute('data-next'));
        navigateToStep(nextStep);
      });
    });

    prevButtons.forEach(button => {
      button.addEventListener('click', function() {
        const prevStep = parseInt(this.getAttribute('data-prev'));
        navigateToStep(prevStep);
      });
    });

    editButtons.forEach(button => {
      button.addEventListener('click', function() {
        const editStep = parseInt(this.getAttribute('data-edit'));
        navigateToStep(editStep);
      });
    });

    // Payment Method Selection for multi-step checkout
    paymentMethods.forEach(header => {
      header.addEventListener('click', function() {
        // Get the radio input within this header
        const radio = this.querySelector('input[type="radio"]');
        if (radio) {
          radio.checked = true;

          // Update active state for all payment methods
          const allPaymentMethods = document.querySelectorAll('.payment-method');
          allPaymentMethods.forEach(method => {
            method.classList.remove('active');
          });

          // Add active class to the parent payment method
          this.closest('.payment-method').classList.add('active');

          // Show/hide payment method bodies
          const allPaymentBodies = document.querySelectorAll('.payment-method-body');
          allPaymentBodies.forEach(body => {
            body.classList.add('d-none');
          });

          const selectedBody = this.closest('.payment-method').querySelector('.payment-method-body');
          if (selectedBody) {
            selectedBody.classList.remove('d-none');
          }
        }
      });
    });

    // Order Summary Toggle (Mobile)
    if (summaryToggle) {
      summaryToggle.addEventListener('click', function() {
        this.classList.toggle('collapsed');

        if (orderSummaryContent) {
          orderSummaryContent.classList.toggle('d-none');
        }

        // Toggle icon
        const icon = this.querySelector('i');
        if (icon) {
          if (icon.classList.contains('bi-chevron-down')) {
            icon.classList.remove('bi-chevron-down');
            icon.classList.add('bi-chevron-up');
          } else {
            icon.classList.remove('bi-chevron-up');
            icon.classList.add('bi-chevron-down');
          }
        }
      });
    }

    // Form Validation for multi-step checkout
    const forms = document.querySelectorAll('.checkout-form-element');
    forms.forEach(form => {
      form.addEventListener('submit', function(e) {
        e.preventDefault();

        // Basic validation
        const requiredFields = form.querySelectorAll('[required]');
        let isValid = true;

        requiredFields.forEach(field => {
          if (!field.value.trim()) {
            isValid = false;
            field.classList.add('is-invalid');
          } else {
            field.classList.remove('is-invalid');
          }
        });

        // If it's the final form and valid, show success message
        if (isValid && form.closest('.checkout-form[data-form="4"]')) {
          // Hide form fields
          const formFields = form.querySelectorAll('.form-group, .review-sections, .form-check, .d-flex');
          formFields.forEach(field => {
            field.style.display = 'none';
          });

          // Show success message
          const successMessage = form.querySelector('.success-message');
          if (successMessage) {
            successMessage.classList.remove('d-none');

            // Add animation
            successMessage.style.animation = 'fadeInUp 0.5s ease forwards';
          }

          // Simulate redirect after 3 seconds
          setTimeout(() => {
            // In a real application, this would redirect to an order confirmation page
            console.log('Redirecting to order confirmation page...');
          }, 3000);
        }
      });
    });

    // Function to navigate between steps
    function navigateToStep(stepNumber) {
      // Update steps
      checkoutSteps.forEach(step => {
        const stepNum = parseInt(step.getAttribute('data-step'));

        if (stepNum < stepNumber) {
          step.classList.add('completed');
          step.classList.remove('active');
        } else if (stepNum === stepNumber) {
          step.classList.add('active');
          step.classList.remove('completed');
        } else {
          step.classList.remove('active', 'completed');
        }
      });

      // Update step connectors
      const connectors = document.querySelectorAll('.step-connector');
      connectors.forEach((connector, index) => {
        if (index + 1 < stepNumber) {
          connector.classList.add('completed');
          connector.classList.remove('active');
        } else if (index + 1 === stepNumber - 1) {
          connector.classList.add('active');
          connector.classList.remove('completed');
        } else {
          connector.classList.remove('active', 'completed');
        }
      });

      // Show the corresponding form
      checkoutForms.forEach(form => {
        const formNum = parseInt(form.getAttribute('data-form'));

        if (formNum === stepNumber) {
          form.classList.add('active');

          // Scroll to top of form on mobile
          if (window.innerWidth < 768) {
            form.scrollIntoView({
              behavior: 'smooth',
              block: 'start'
            });
          }
        } else {
          form.classList.remove('active');
        }
      });
    }
  }

  // Function to initialize one-page checkout
  function initOnePageCheckout() {
    // Payment Method Selection for one-page checkout
    const paymentOptions = document.querySelectorAll('.payment-option input[type="radio"]');

    paymentOptions.forEach(option => {
      option.addEventListener('change', function() {
        // Update active class on payment options
        document.querySelectorAll('.payment-option').forEach(opt => {
          opt.classList.remove('active');
        });

        this.closest('.payment-option').classList.add('active');

        // Show/hide payment details
        const paymentId = this.id;
        document.querySelectorAll('.payment-details').forEach(details => {
          details.classList.add('d-none');
        });

        document.getElementById(`${paymentId}-details`).classList.remove('d-none');
      });
    });

    // Form Validation for one-page checkout
    const checkoutForm = document.querySelector('.checkout-form');

    if (checkoutForm) {
      checkoutForm.addEventListener('submit', function(e) {
        e.preventDefault();

        // Basic validation
        const requiredFields = checkoutForm.querySelectorAll('[required]');
        let isValid = true;

        requiredFields.forEach(field => {
          if (!field.value.trim()) {
            isValid = false;
            field.classList.add('is-invalid');

            // Scroll to first invalid field
            if (isValid === false) {
              field.scrollIntoView({
                behavior: 'smooth',
                block: 'center'
              });
              field.focus();
              isValid = null; // Set to null so we only scroll to the first invalid field
            }
          } else {
            field.classList.remove('is-invalid');
          }
        });

        // If form is valid, show success message
        if (isValid === true) {
          // Hide form sections except the last one
          const sections = document.querySelectorAll('.checkout-section');
          sections.forEach((section, index) => {
            if (index < sections.length - 1) {
              section.style.display = 'none';
            }
          });

          // Hide terms checkbox and place order button
          const termsCheck = document.querySelector('.terms-check');
          const placeOrderContainer = document.querySelector('.place-order-container');

          if (termsCheck) termsCheck.style.display = 'none';
          if (placeOrderContainer) placeOrderContainer.style.display = 'none';

          // Show success message
          const successMessage = document.querySelector('.success-message');
          if (successMessage) {
            successMessage.classList.remove('d-none');
            successMessage.style.animation = 'fadeInUp 0.5s ease forwards';
          }

          // Scroll to success message
          const orderReview = document.getElementById('order-review');
          if (orderReview) {
            orderReview.scrollIntoView({
              behavior: 'smooth',
              block: 'start'
            });
          }

          // Simulate redirect after 3 seconds
          setTimeout(() => {
            // In a real application, this would redirect to an order confirmation page
            console.log('Redirecting to order confirmation page...');
          }, 3000);
        }
      });

      // Add input event listeners to clear validation styling when user types
      const formInputs = checkoutForm.querySelectorAll('input, select, textarea');
      formInputs.forEach(input => {
        input.addEventListener('input', function() {
          if (this.value.trim()) {
            this.classList.remove('is-invalid');
          }
        });
      });
    }
  }

  // Function to initialize input masks (common for both checkout types)
  function initInputMasks() {
    // Card number input mask (format: XXXX XXXX XXXX XXXX)
    const cardNumberInput = document.getElementById('card-number');
    if (cardNumberInput) {
      cardNumberInput.addEventListener('input', function(e) {
        let value = e.target.value.replace(/\D/g, '');
        if (value.length > 16) value = value.slice(0, 16);

        // Add spaces after every 4 digits
        let formattedValue = '';
        for (let i = 0; i < value.length; i++) {
          if (i > 0 && i % 4 === 0) {
            formattedValue += ' ';
          }
          formattedValue += value[i];
        }

        e.target.value = formattedValue;
      });
    }

    // Expiry date input mask (format: MM/YY)
    const expiryInput = document.getElementById('expiry');
    if (expiryInput) {
      expiryInput.addEventListener('input', function(e) {
        let value = e.target.value.replace(/\D/g, '');
        if (value.length > 4) value = value.slice(0, 4);

        // Format as MM/YY
        if (value.length > 2) {
          value = value.slice(0, 2) + '/' + value.slice(2);
        }

        e.target.value = value;
      });
    }

    // CVV input mask (3-4 digits)
    const cvvInput = document.getElementById('cvv');
    if (cvvInput) {
      cvvInput.addEventListener('input', function(e) {
        let value = e.target.value.replace(/\D/g, '');
        if (value.length > 4) value = value.slice(0, 4);
        e.target.value = value;
      });
    }

    // Phone number input mask
    const phoneInput = document.getElementById('phone');
    if (phoneInput) {
      phoneInput.addEventListener('input', function(e) {
        let value = e.target.value.replace(/\D/g, '');
        if (value.length > 10) value = value.slice(0, 10);

        // Format as (XXX) XXX-XXXX
        if (value.length > 0) {
          if (value.length <= 3) {
            value = '(' + value;
          } else if (value.length <= 6) {
            value = '(' + value.slice(0, 3) + ') ' + value.slice(3);
          } else {
            value = '(' + value.slice(0, 3) + ') ' + value.slice(3, 6) + '-' + value.slice(6);
          }
        }

        e.target.value = value;
      });
    }

    // ZIP code input mask (5 digits)
    const zipInput = document.getElementById('zip');
    if (zipInput) {
      zipInput.addEventListener('input', function(e) {
        let value = e.target.value.replace(/\D/g, '');
        if (value.length > 5) value = value.slice(0, 5);
        e.target.value = value;
      });
    }
  }

  // Function to handle promo code application (common for both checkout types)
  function initPromoCode() {
    const promoInput = document.querySelector('.promo-code input');
    const promoButton = document.querySelector('.promo-code button');

    if (promoInput && promoButton) {
      promoButton.addEventListener('click', function() {
        const promoCode = promoInput.value.trim();

        if (promoCode) {
          // Simulate promo code validation
          // In a real application, this would make an API call to validate the code

          // For demo purposes, let's assume "DISCOUNT20" is a valid code
          if (promoCode.toUpperCase() === 'DISCOUNT20') {
            // Show success state
            promoInput.classList.add('is-valid');
            promoInput.classList.remove('is-invalid');
            promoButton.textContent = 'Applied';
            promoButton.disabled = true;

            // Update order total (in a real app, this would recalculate based on the discount)
            const orderTotal = document.querySelector('.order-total span:last-child');
            const btnPrice = document.querySelector('.btn-price');

            if (orderTotal) {
              // Apply a 20% discount
              const currentTotal = parseFloat(orderTotal.textContent.replace('$', ''));
              const discountedTotal = (currentTotal * 0.8).toFixed(2);
              orderTotal.textContent = '$' + discountedTotal;

              // Update button price if it exists
              if (btnPrice) {
                btnPrice.textContent = '$' + discountedTotal;
              }

              // Add discount line
              const orderTotals = document.querySelector('.order-totals');
              if (orderTotals) {
                const discountElement = document.createElement('div');
                discountElement.className = 'order-discount d-flex justify-content-between';
                discountElement.innerHTML = `
                <span>Discount (20%)</span>
                <span>-$${(currentTotal * 0.2).toFixed(2)}</span>
              `;

                // Insert before the total
                const totalElement = document.querySelector('.order-total');
                if (totalElement) {
                  orderTotals.insertBefore(discountElement, totalElement);
                }
              }
            }
          } else {
            // Show error state
            promoInput.classList.add('is-invalid');
            promoInput.classList.remove('is-valid');

            // Reset after 3 seconds
            setTimeout(() => {
              promoInput.classList.remove('is-invalid');
            }, 3000);
          }
        }
      });
    }
  }

  // Function to initialize Bootstrap tooltips
  function initTooltips() {
    // Check if Bootstrap's tooltip function exists
    if (typeof bootstrap !== 'undefined' && typeof bootstrap.Tooltip !== 'undefined') {
      const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
      const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
    } else {
      // Fallback for when Bootstrap JS is not loaded
      const cvvHint = document.querySelector('.cvv-hint');
      if (cvvHint) {
        cvvHint.addEventListener('mouseenter', function() {
          this.setAttribute('data-original-title', this.getAttribute('title'));
          this.setAttribute('title', '');
        });

        cvvHint.addEventListener('mouseleave', function() {
          this.setAttribute('title', this.getAttribute('data-original-title'));
        });
      }
    }
  }

  /**
   * Initiate Pure Counter
   */
  new PureCounter();

  /**
   * Frequently Asked Questions Toggle
   */
  document.querySelectorAll('.faq-item h3, .faq-item .faq-toggle').forEach((faqItem) => {
    faqItem.addEventListener('click', () => {
      faqItem.parentNode.classList.toggle('faq-active');
    });
  });

})();