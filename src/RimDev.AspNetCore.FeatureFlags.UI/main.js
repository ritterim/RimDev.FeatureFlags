const featuresContainer = document.querySelector('#features');
const notificationsContainer = document.querySelector('#notifications-container');

const fetchOptions = {
  credentials: 'same-origin'
};

fetch('/_features/get_all', fetchOptions)
  .then(res => res.json())
  .then(json => {
    const features = json.map(feature => `<li class="mdl-list__item mdl-list__item--three-line">
      <span class="mdl-list__item-primary-content">
        <i class="material-icons mdl-list__item-avatar">outlined_flag</i>
        <span>${feature.name}</span>
        ${feature.description ? '<span class="mdl-list__item-text-body">' + feature.description + '</span>' : ''}
      </span>
      <span class="mdl-list__item-secondary-content">
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
      </span>
    </li>`);

    featuresContainer.innerHTML = DOMPurify.sanitize(features.join(''));

    [...featuresContainer.getElementsByClassName('mdl-js-radio')].forEach(toggle => {
      componentHandler.upgradeElement(toggle);
    });

    document.querySelectorAll('input[type="radio"]').forEach(radio => {
      radio.addEventListener('change', evt => {
        const feature = evt.currentTarget.getAttribute('data-feature');
        const checked = evt.currentTarget.getAttribute('data-checked');

        console.log(checked);
        console.log(feature);

        fetch('/_features/set', {
          method: 'POST',
          body: JSON.stringify({
            name: feature,
            enabled: checked
          }),
          headers: { 'Content-Type': 'application/json' },
          ...fetchOptions
        }).then(() => {
          notificationsContainer.MaterialSnackbar.showSnackbar({
            message: `${feature} set to ${checked}`
          });
        }).catch(err => {
          const toggle = document.getElementById(feature).parentElement;

          notificationsContainer.MaterialSnackbar.showSnackbar({
            message: `ERROR: ${err}`
          });
        });
      });
    });
  })
  .catch(err => {
    notificationsContainer.MaterialSnackbar.showSnackbar({
      message: `ERROR: ${err}`
    });
  });
