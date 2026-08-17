// A friendly, non-blocking touch: when you land on the Heritage Books category, the assistant
// pops in on its own to say hello. It doesn't gate anything — the books are already on the page.
(function () {
    const panel = document.getElementById('aiPanel');
    const body = document.getElementById('aiBody');
    if (!panel || !body) return;

    panel.classList.add('open');

    const div = document.createElement('div');
    div.className = 'ai-msg bot';
    div.textContent = "📚 Welcome to Heritage Books! I'm the Heritage Guide — curious about a country's literary heritage? Ask me anything, or just browse below.";
    body.appendChild(div);
    body.scrollTop = body.scrollHeight;
})();
