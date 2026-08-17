(function () {
    const dataEl = document.getElementById('books-gate-data');
    if (!dataEl) return;
    const data = JSON.parse(dataEl.textContent);

    const panel = document.getElementById('aiPanel');
    const body = document.getElementById('aiBody');
    const suggestions = document.getElementById('aiSuggestions');
    const input = document.getElementById('aiInput');
    const sendBtn = document.getElementById('aiSendBtn');
    if (!panel || !body) return;

    let step = 'intro';
    const captured = { country: null, reason: null };

    function botSay(text) {
        const div = document.createElement('div');
        div.className = 'ai-msg bot';
        div.textContent = text;
        body.appendChild(div);
        body.scrollTop = body.scrollHeight;
        return div;
    }

    function userSay(text) {
        const div = document.createElement('div');
        div.className = 'ai-msg user';
        div.textContent = text;
        body.appendChild(div);
        body.scrollTop = body.scrollHeight;
    }

    function botSayHtml(html) {
        const div = document.createElement('div');
        div.className = 'ai-msg bot';
        div.innerHTML = html;
        body.appendChild(div);
        body.scrollTop = body.scrollHeight;
    }

    function showTyping() {
        const div = document.createElement('div');
        div.className = 'ai-msg bot typing';
        div.innerHTML = '<span></span><span></span><span></span>';
        body.appendChild(div);
        body.scrollTop = body.scrollHeight;
        return div;
    }

    function showCountryChips() {
        suggestions.innerHTML = '';
        suggestions.style.display = 'flex';
        data.countries.forEach((country) => {
            const chip = document.createElement('button');
            chip.type = 'button';
            chip.className = 'ai-chip';
            chip.textContent = country;
            chip.addEventListener('click', () => selectCountry(country));
            suggestions.appendChild(chip);
        });
    }

    function hideChips() {
        suggestions.innerHTML = '';
        suggestions.style.display = 'none';
    }

    function selectCountry(country) {
        if (step !== 'askCountry') return;
        captured.country = country;
        userSay(country);
        hideChips();
        step = 'askReason';
        setTimeout(() => botSay(`${country}'s heritage runs deep in its books. Why are you interested in reading?`), 350);
    }

    function getAntiForgeryToken() {
        const el = document.querySelector('#antiForgeryForm input[name="__RequestVerificationToken"]');
        return el ? el.value : '';
    }

    async function handleReasonReply(text) {
        step = 'submitting';
        const typing = showTyping();

        // Ask the live Heritage Guide for a warm, relevant reply — falls back gracefully if no
        // live model is configured, same as the general assistant widget elsewhere on the site.
        try {
            const chatResponse = await fetch('/Assistant/Ask', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': getAntiForgeryToken() },
                body: JSON.stringify({ message: text, history: [] })
            });
            typing.remove();
            if (chatResponse.ok) {
                const chatData = await chatResponse.json();
                botSay(chatData.reply);
            }
        } catch {
            typing.remove();
        }

        await submitRequest();
    }

    async function submitRequest() {
        const submitTyping = showTyping();
        try {
            const response = await fetch('/Products/SubmitBookAccessRequestAjax', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `preferredCountry=${encodeURIComponent(captured.country)}&reason=${encodeURIComponent(captured.reason)}&__RequestVerificationToken=${encodeURIComponent(getAntiForgeryToken())}`
            });
            submitTyping.remove();
            if (response.ok) {
                botSay('Your Heritage Books shelf is unlocked — enjoy! Taking you there now. 🎉');
                setTimeout(() => { window.location.href = '/Products/Books'; }, 1400);
            } else {
                botSay("Hmm, I couldn't send that just now. Please try again in a moment.");
            }
        } catch {
            submitTyping.remove();
            botSay("Hmm, I couldn't send that just now. Please try again in a moment.");
        }
    }

    function handleSend() {
        const text = input.value.trim();
        if (!text || step !== 'askReason') return;
        input.value = '';
        captured.reason = text;
        userSay(text);
        handleReasonReply(text);
    }

    // Auto-open the assistant — this page's whole point is the guided chat, not a form.
    panel.classList.add('open');

    if (!data.isAuthenticated) {
        botSay("📚 Oh, you're interested in Heritage Books! I'm the Heritage Guide.");
        setTimeout(() => {
            botSayHtml(
                'Before we start, I need to know who you are so I know whose shelf to unlock. ' +
                '<div class="mt-2"><a href="/Identity/Account/Login?returnUrl=/Products/Books" class="btn btn-heritage btn-sm me-2">Log In</a>' +
                '<a href="/Identity/Account/Register?returnUrl=/Products/Books" class="btn btn-outline-heritage btn-sm">Register</a></div>'
            );
        }, 400);
        return;
    }

    if (data.status === 'Pending') {
        botSay('📚 Welcome back! Your request is already with our team — I\'ll unlock the shelf for you the moment an admin approves it.');
        return;
    }

    sendBtn.addEventListener('click', handleSend);
    input.addEventListener('keydown', (e) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            handleSend();
        }
    });

    botSay("📚 Oh, you're interested in Heritage Books! I'm the Heritage Guide.");

    const openingDelay = data.status === 'Rejected' ? 350 : 0;
    if (data.status === 'Rejected') {
        setTimeout(() => {
            botSay(data.adminNote
                ? `Your last request wasn't approved: "${data.adminNote}". Let's try again.`
                : "Your last request wasn't approved. Let's try again.");
        }, openingDelay);
    }

    setTimeout(() => {
        step = 'askCountry';
        botSay("Which country's books and heritage do you love most?");
        showCountryChips();
    }, openingDelay + 500);
})();
