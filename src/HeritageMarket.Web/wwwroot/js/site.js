// Wishlist heart toggle (AJAX, no page reload)
document.addEventListener('click', function (e) {
    const btn = e.target.closest('.wish-heart-btn');
    if (!btn) return;
    e.preventDefault();

    const productId = btn.getAttribute('data-product-id');
    const tokenInput = document.querySelector('#antiForgeryForm input[name="__RequestVerificationToken"]');
    if (!tokenInput) return;

    btn.disabled = true;
    fetch('/Wishlist/Toggle', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `productId=${encodeURIComponent(productId)}&__RequestVerificationToken=${encodeURIComponent(tokenInput.value)}`
    })
        .then(r => {
            if (!r.ok) throw new Error('Request failed');
            return r.json();
        })
        .then(data => {
            btn.classList.toggle('active', data.wishlisted);
            btn.title = data.wishlisted ? 'Remove from wishlist' : 'Add to wishlist';
            const icon = btn.querySelector('i');
            icon.className = data.wishlisted ? 'bi bi-heart-fill' : 'bi bi-heart';
            updateWishlistBadge(data.wishlisted ? 1 : -1);
        })
        .catch(() => { /* silently ignore; the heart just won't update */ })
        .finally(() => { btn.disabled = false; });
});

function updateWishlistBadge(delta) {
    const badge = document.getElementById('wishlistCountBadge');
    if (!badge) return;
    const next = Math.max(0, (parseInt(badge.textContent, 10) || 0) + delta);
    badge.textContent = next;
    badge.classList.toggle('d-none', next === 0);
}
