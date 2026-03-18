window.initRazorpay = (options) => {
    return new Promise((resolve) => {

        const waitForRazorpay = (retries = 10) => {
            if (typeof Razorpay !== 'undefined') {
                openRazorpay(options, resolve);
            } else if (retries > 0) {
                setTimeout(() => waitForRazorpay(retries - 1), 300);
            } else {
                console.error('Razorpay script failed to load.');
                resolve({ success: false, paymentId: null });
            }
        };

        waitForRazorpay();
    });
};

function openRazorpay(options, resolve) {
    const rzpOptions = {
        ...options,
        handler: function (response) {
            resolve({
                success: true,
                paymentId: response.razorpay_payment_id,
                orderId: response.razorpay_order_id,
                signature: response.razorpay_signature
            });
        },
        modal: {
            ondismiss: function () {
                resolve({ success: false, paymentId: null });
            },
            escape: true,
            backdropclose: false,
            handleback: true,
            confirm_close: false,
        }
    };

    try {
        const rzp = new Razorpay(rzpOptions);

        rzp.on('payment.failed', function (response) {
            resolve({ success: false, paymentId: null });
        });

        try {
            rzp.open();
        } catch (openErr) {
            console.error('Failed to open Razorpay checkout:', openErr);
            resolve({ success: false, paymentId: null });
        }
    } catch (e) {
        console.error('Razorpay init error:', e);
        resolve({ success: false, paymentId: null });
    }
}