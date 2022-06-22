const featuresContainer = document.querySelector('#features-list');
const messageContainer = document.querySelector('.js-message-container');
const message = messageContainer.querySelector('.js-message');
const messageText = message.querySelector('.js-message-text');

const fetchOptions = {
  credentials: 'same-origin'
};

var hideMessageContainer = () => {
  messageContainer.classList.remove('pin-bottom');
}

var removeMessage = (type) => {
  message.classList.remove('message--'+type+'');
  messageText.innerHTML = "";
}

let clearMessage;

var showMessage = (text, type) => {
  messageContainer.classList.add('pin-bottom');
  message.classList.add('message--'+type+'');
  messageText.innerHTML = text;
}

var handleMessage = (text, type) => {
  showMessage(text, type);
  clearMessage = setTimeout(() => {
    hideMessageContainer();
    setTimeout(removeMessage, 300);
  }, 3000);
}

var fireMessage = (text, type) => {

  if(messageContainer.classList.contains('pin-bottom')) {
    hideMessageContainer();

    clearTimeout(clearMessage);
    
    setTimeout(() => {
      removeMessage(type);
      handleMessage(text, type);
    }, 300);
  } else {
    handleMessage(text, type);
  }
}

fetch('/_features/get_all', fetchOptions)
  .then(res => res.json())
  .then(json => {
    const features = json.map(feature => `<li class="block-container w-100 mb-4">
      <div class="block block-6">
        <div class="flex flex--align-center">
          <div class="flex--center-content p-3 mr-2 background--light inverted pill--circle-large">
            <i class="pi-flag pi-xl"></i>
          </div>
          <div>
            <strong>${feature.name}</strong>
            <p class="mb-0">${feature.description ? '<span class="">' + feature.description + '</span>' : ''}</p>
          </div>
        </div>
      </div>
      <div class="block block-6 flex flex--align-center form">
        <fieldset class="form__field flex">
          <legend class="sr-only">Set the flag</legend>
          <input id="${feature.name}-null" type="radio" data-feature="${feature.name}" data-checked="null" name="${feature.name}" value="1" ${feature.enabled == null ? " checked" : ""}>
          <label for="${feature.name}-null">
            <div class="input-icons">
              <i class="pi-circle pi-lg"></i>
              <i class="pi-circle-solid"></i>
            </div>
            Null
          </label>
          <input id="${feature.name}-false" type="radio" data-feature="${feature.name}" data-checked="false" name="${feature.name}" value="1" ${feature.enabled == false ? " checked" : ""}>
          <label for="${feature.name}-false">
            <div class="input-icons">
              <i class="pi-circle pi-lg"></i>
              <i class="pi-circle-solid"></i>
            </div>
            False
          </label>
          <input id="${feature.name}-true" type="radio" data-feature="${feature.name}" data-checked="true" name="${feature.name}" value="1" ${feature.enabled == true ? " checked" : ""}>
          <label for="${feature.name}-true">
            <div class="input-icons">
              <i class="pi-circle pi-lg"></i>
              <i class="pi-circle-solid"></i>
            </div>
            True
          </label>
        </fieldset>
      </div>
    </li>`);

    featuresContainer.innerHTML = DOMPurify.sanitize(features.join(''));

    document.querySelectorAll('input[type="radio"]').forEach(radio => {
      radio.addEventListener('change', evt => {
        const feature = evt.currentTarget.getAttribute('data-feature');
        const checked = evt.currentTarget.getAttribute('data-checked');

        fetch('/_features/set', {
          method: 'POST',
          body: JSON.stringify({
            name: feature,
            enabled: checked
          }),
          headers: { 'Content-Type': 'application/json' },
          ...fetchOptions
        }).then(() => {
          let message = `${feature} set to ${checked}`;

          fireMessage(message, 'success');
        }).catch(err => {
          let message = `ERROR: ${err}`

          fireMessage(message, 'error');
        });
      });
    });
  })
  .catch(err => {
    let message = `ERROR: ${err}`
    fireMessage(message, 'error');
  });
