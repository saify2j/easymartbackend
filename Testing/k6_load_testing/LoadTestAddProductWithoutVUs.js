import http from 'k6/http';
import { check } from 'k6';

// Options: 1 VU, but iterate 300 times
export const options = {
    vus: 1,
    iterations: 300, // total 300 requests
};

// Simple random string generator for productName
function randomProductName(length = 8) {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
    let result = '';
    for (let i = 0; i < length; i++) {
        result += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return result;
}

export default function () {
    const url = 'http://localhost:5211/Product/AddProduct';
    const payload = JSON.stringify({
        productName: randomProductName(),
        price: 250
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
            'accept': 'text/plain'
        },
    };

    const res = http.post(url, payload, params);

    // Optional: check for success
    check(res, {
        'status is 200': (r) => r.status === 200,
    });
}