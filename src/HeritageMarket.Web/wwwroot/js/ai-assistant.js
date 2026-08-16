(function () {
    const openBtn = document.getElementById('aiOpenBtn');
    const closeBtn = document.getElementById('aiCloseBtn');
    const panel = document.getElementById('aiPanel');
    const body = document.getElementById('aiBody');
    const input = document.getElementById('aiInput');
    const sendBtn = document.getElementById('aiSendBtn');
    const suggestions = document.getElementById('aiSuggestions');

    if (!openBtn || !panel) return;

    let history = [];
    let sending = false;

    openBtn.addEventListener('click', () => panel.classList.add('open'));
    closeBtn.addEventListener('click', () => panel.classList.remove('open'));

    function appendMessage(role, text) {
        const div = document.createElement('div');
        div.className = `ai-msg ${role}`;
        div.textContent = text;
        body.appendChild(div);
        body.scrollTop = body.scrollHeight;
        return div;
    }

    function getAntiForgeryToken() {
        const el = document.querySelector('#antiForgeryForm input[name="__RequestVerificationToken"]');
        return el ? el.value : '';
    }

    async function send(text) {
        if (sending || !text) return;
        sending = true;
        suggestions.style.display = 'none';
        appendMessage('user', text);
        history.push({ role: 'user', content: text });

        const typing = document.createElement('div');
        typing.className = 'ai-msg bot typing';
        typing.innerHTML = '<span></span><span></span><span></span>';
        body.appendChild(typing);
        body.scrollTop = body.scrollHeight;

        try {
            const response = await fetch('/Assistant/Ask', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-CSRF-TOKEN': getAntiForgeryToken()
                },
                body: JSON.stringify({ message: text, history: history.slice(-6) })
            });

            typing.remove();

            if (!response.ok) throw new Error('Assistant request failed');

            const data = await response.json();
            appendMessage('bot', data.reply);
            history.push({ role: 'assistant', content: data.reply });
        } catch {
            typing.remove();
            appendMessage('bot', "Sorry, I couldn't reach the assistant just now. Please try again in a moment.");
        } finally {
            sending = false;
        }
    }

    sendBtn.addEventListener('click', () => {
        const text = input.value.trim();
        input.value = '';
        send(text);
    });

    input.addEventListener('keydown', (e) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            const text = input.value.trim();
            input.value = '';
            send(text);
        }
    });

    suggestions.querySelectorAll('.ai-chip').forEach((chip) => {
        chip.addEventListener('click', () => send(chip.getAttribute('data-ask')));
    });
})();
