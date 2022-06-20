const featuresContainer = document.querySelector('#features-list');
const notificationsContainer = document.querySelector('#notifications-container');

const fetchOptions = {
  credentials: 'same-origin'
};

fetch('/_features/get_all', fetchOptions)
  .then(res => res.json())
  .then(json => {
    const features = json.map(feature => `<li class="block-container w-100">
      <div class="block block-6 flex">
        <div class="flex flex--align-center">
          <div class="flex--center-content p-3 background--light inverted pill--circle-large">
            <i class="pi-flag"></i>
          </div>
          <p>${feature.name}</p>
        </div>
        <p>${feature.description ? '<span class="mdl-list__item-text-body">' + feature.description + '</span>' : ''}</p>
      </div>
      <div class="block block-6">
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

    [...featuresContainer.getElementsByClassName('mdl-js-radio')].forEach(toggle => {
      componentHandler.upgradeElement(toggle);
    });

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
          let message = `${feature} set to ${checked}`
          alert(message);
          // notificationsContainer.MaterialSnackbar.showSnackbar({
          //   message: `${feature} set to ${checked}`
          // });
        }).catch(err => {
          const toggle = document.getElementById(feature).parentElement;
          let message = `ERROR: ${err}`
          alert(message);

          // notificationsContainer.MaterialSnackbar.showSnackbar({
          //   message: `ERROR: ${err}`
          // });
        });
      });
    });
  })
  .catch(err => {
    let message = `ERROR: ${err}`
    alert(message);
    // notificationsContainer.MaterialSnackbar.showSnackbar({
    //   message: `ERROR: ${err}`
    // });
  });
