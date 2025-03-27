document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('button[data-url]').forEach(button => {
        button.addEventListener('click', function (event) {
            event.preventDefault();

            const url = this.getAttribute('data-url');
            const method = this.getAttribute('data-method') || 'get';

            console.log(`Button clicked: URL=${url}, Method=${method}`);

            if (method.toLowerCase() === 'post') {
                const form = this.closest('form');
                if (form) {
                    fetch(url, {
                        method: 'POST',
                        body: new FormData(form),
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest'
                        }
                    })
                        .then(response => {
                            if (response.redirected) {
                                window.location.href = response.url;
                            } else if (response.ok) {
                                return response.text();
                            } else {
                                throw new Error('Request failed');
                            }
                        })
                        .then(data => {
                            if (data) {
                                document.open();
                                document.write(data);
                                document.close();
                            }
                        })
                        .catch(error => {
                            console.error('Error:', error);
                            alert('Произошла ошибка при выполнении запроса.');
                        });
                }
            } else {
                window.location.href = url;
            }
        });
    });
});