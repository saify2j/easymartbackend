import http from 'k6/http';
import { check, sleep } from 'k6';

// Load test options
export const options = {
    vus: 10, // 10 virtual users
    duration: '30s', // run for 30 seconds
};

// Random product name generator
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
        price: 250,
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
            'accept': 'text/plain',
        },
    };

    const res = http.post(url, payload, params);

    // Optional: check if request succeeded
    check(res, {
        'status is 200': (r) => r.status === 200,
    });
}