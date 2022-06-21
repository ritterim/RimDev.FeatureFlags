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
    const features = json.map(feature => `<li class="block-container w-100">
      <div class="block block-6">
        <div class="flex flex--align-center">
          <div class="flex--center-content p-3 mr-2 background--light inverted pill--circle-large">
            <i class="pi-flag pi-xl"></i>
          </div>
          <div>
            <strong>${feature.name}</strong>
            <p class="mb-0">${feature.description ? '<span class="mdl-list__item-text-body">' + feature.description + '</span>' : ''}</p>
          </div>
        </div>
      </div>
      <div class="block block-6 flex flex--align-center">
        <fieldset class="mdl-list__item-secondary-action" id="">
          <legend class="hidden">Set the flag</legend>
          <label class="mdl-radio mdl-js-radio mdl-js-ripple-effect" for="${feature.name}-null">
            <input type="radio" id="${feature.name}-null" class="mdl-radio__button" data-feature="${feature.name}" data-checked="null" name="${feature.name}" value="1" ${feature.enabled == null ? " checked" : ""}>
            <span class="mdl-radio__label">Null</span>
          </label>
          <label class="mdl-radio mdl-js-radio mdl-js-ripple-effect" for="${feature.name}-false">
            <input type="radio" id="${feature.name}-false" class="mdl-radio__button" data-feature="${feature.name}" data-checked="false" name="${feature.name}" value="1" ${feature.enabled == false ? " checked" : ""}>
            <span class="mdl-radio__label">False</span>
          </label>
          <label class="mdl-radio mdl-js-radio mdl-js-ripple-effect" for="${feature.name}-true">
            <input type="radio" id="${feature.name}-true" class="mdl-radio__button" data-feature="${feature.name}" data-checked="true" name="${feature.name}" value="1" ${feature.enabled == true ? " checked" : ""}>
            <span class="mdl-radio__label">True</span>
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
