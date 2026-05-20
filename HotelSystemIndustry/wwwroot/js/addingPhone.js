let phoneIndex = 0;
function addPhone() {
    const list = document.getElementById('phoneList');
    const entry = document.createElement('div');
    entry.className = 'phone-entry';
    entry.innerHTML = `
                <input type="text"
                       name="PhoneNumbers[${phoneIndex}]"
                       class="form-control"
                       placeholder="+48 22 000 00 00" />
                <button type="button" class="btn-remove-phone" onclick="removePhone(this)" title="Usuń">&#215;</button>
            `;
    list.appendChild(entry);
    phoneIndex++;
    reindexPhones();
}
function removePhone(btn) {
    const list = document.getElementById('phoneList');

    if (list.children.length === 1)
    {
        btn.previousElementSibling.value = '';
        return;
    }
    btn.parentElement.remove();
    reindexPhones();
}

function reindexPhones()
{
    const inputs = document.querySelectorAll('#phoneList input[type="text"]');
    inputs.forEach(
        (input, i) =>
        {
            input.name = `PhoneNumbers[${i}]`;
        }
    );
    phoneIndex = inputs.length;
}